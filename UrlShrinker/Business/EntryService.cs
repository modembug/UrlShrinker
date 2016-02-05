using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using UrlShrinker.Models;

namespace UrlShrinker.Business
{
    public sealed class EntryService : IEntryService
    {
        private ITokenService _tokenService;

        public EntryService()
        {
            _tokenService = new TokenService();
        }

        private string GetFilterType(QueryType queryType)
        {
            switch (queryType)
            {
                case QueryType.Token:
                    return "Url";
                case QueryType.Url:
                    return "Token";
                default:
                    throw new Exception("Unknown QueryType encountered.");
            }
        }

        private async Task<string> GetConnectionString()
        {
            //Based on guidelines found here: https://support.appharbor.com/kb/add-ons/using-sequelizer
            return await Task.Run(() =>
            {
                Uri uri = new Uri(ConfigurationManager.AppSettings["SQLSERVER_URI"]);

                return new SqlConnectionStringBuilder
                {
                    DataSource = uri.Host,
                    InitialCatalog = uri.AbsolutePath.Trim('/'),
                    UserID = uri.UserInfo.Split(':').First(),
                    Password = uri.UserInfo.Split(':').Last(),
                }.ConnectionString;
            });
        }

        private string BuildShortUrl(IHomeModel model)
        {
            return String.Format("http://{0}/{1}", model.Host, model.Token);
        }

        public async Task<string> AddEntry(IHomeModel model)
        {
            model.Token = await _tokenService.GetToken();

            if (await Exists(model, QueryType.Token))
            {
                //Generate new token and try again.
                model.Token = await _tokenService.GetToken();
                return await AddEntry(model);
            }

            if (await Exists(model, QueryType.Url))
            {
                //Url already exists, return existing token.
                model.Token = await GetEntry(model, QueryType.Token);
                return BuildShortUrl(model);
            }

            return await Task.Run(async () =>
            {
                using (SqlConnection con = new SqlConnection(await GetConnectionString()))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand("INSERT INTO [dbo].[Entry]([Url], [Token]) VALUES(@URL, @Token);", con))
                    {
                        command.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "URL",
                            Value = model.Url,
                        });
                        command.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "Token",
                            Value = model.Token,
                        });

                        await command.ExecuteNonQueryAsync();
                        return BuildShortUrl(model);
                    }
                }
            });
        }

        public async Task DeleteEntry(IHomeModel model, QueryType queryType)
        {
            await Task.Run(async () =>
            {
                string qt = Enum.GetName(typeof(QueryType), queryType);

                using (SqlConnection con = new SqlConnection(await GetConnectionString()))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand(string.Format("DELETE FROM [dbo].[Entry] WHERE [{0}] = @{0};", qt), con))
                    {
                        command.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = qt,
                            Value = model.GetType().GetProperty(qt).GetValue(model, null)
                        });

                        await command.ExecuteNonQueryAsync();
                    }
                }
            });
        }

        public async Task<string> GetEntry(IHomeModel model, QueryType queryType)
        {
            return await Task.Run(async () =>
            {
                string valueType = Enum.GetName(typeof(QueryType), queryType);

                string filterType = GetFilterType(queryType);

                using (SqlConnection con = new SqlConnection(await GetConnectionString()))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand(string.Format("SELECT [{0}] FROM [dbo].[Entry] WHERE [{1}] = @{1}", valueType, filterType), con))
                    {
                        command.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = filterType,
                            Value = model.GetType().GetProperty(filterType).GetValue(model, null)
                        });

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                return reader.GetString(0);
                            }
                            throw new Exception(string.Format("{0} {1} not found in database.", filterType, valueType));
                        }
                    }
                }
            });
        }

        public async Task<bool> Exists(IHomeModel model, QueryType queryType)
        {
            return await Task.Run(async () =>
            {
                bool exists = true;
                string qt = Enum.GetName(typeof(QueryType), queryType);

                using (SqlConnection con = new SqlConnection(await GetConnectionString()))
                {
                    con.Open();

                    using (
                        SqlCommand command =
                            new SqlCommand(
                                string.Format("if (EXISTS (SELECT [Id] FROM [dbo].[Entry] WHERE [{0}] = @{0})) SELECT 'true' ELSE SELECT 'false'", qt),
                                con))
                    {

                        command.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = qt,
                            Value = model.GetType().GetProperty(qt).GetValue(model, null)
                        });

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                exists = bool.Parse(reader.GetString(0));
                            }
                            return exists;
                        }
                    }
                }
            });
        }

        public async Task CreateEntryStorage()
        {
            await Task.Run(async () =>
            {
                using (SqlConnection con = new SqlConnection(await GetConnectionString()))
                {
                    con.Open();
                    //Check if table exists already
                    using (SqlCommand command = new SqlCommand("IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Entry')) SELECT 'true' ELSE SELECT 'false'", con))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                //Table exists so do nothing more.
                                if (bool.Parse(reader.GetString(0))) return;
                            }
                        }
                    }

                    //Create Entry Table
                    using (SqlCommand command = new SqlCommand("CREATE TABLE [dbo].[Entry] ([Id] [int] IDENTITY(1,1) NOT NULL, [Url] [varchar](2048) NOT NULL, [Token] [varchar](255) NOT NULL, PRIMARY KEY CLUSTERED ([Id] ASC))", con))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    //Create index for Token column.
                    using (SqlCommand command = new SqlCommand("CREATE NONCLUSTERED INDEX IDX_Token ON [dbo].[Entry]([Token])", con))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            });
        }
    }
}