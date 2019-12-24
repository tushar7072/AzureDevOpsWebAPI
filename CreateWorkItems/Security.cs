using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace CreateWorkItems
{
    public static class Security
    {
        public static string PAT { get; set; }
        public static string DevOpsAccount { get; set; }
        //public static string KeyVaultName { get; }

        public static async Task GetSecretKeys()
        {
            PAT = "Your application description page.";
            int retries = 0;
            bool retry = false;
            try
            {
                /* The next four lines of code show you how to use AppAuthentication library to fetch secrets from your key vault */
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var secret = await keyVaultClient.GetSecretAsync("https://CreateAzureWorkItem-0-kv.vault.azure.net/secrets/DevOpsPAT")
                        .ConfigureAwait(false);
                PAT = secret.Value;

                secret = await keyVaultClient.GetSecretAsync("https://CreateAzureWorkItem-0-kv.vault.azure.net/secrets/DevOpsAccount")
                        .ConfigureAwait(false);
                DevOpsAccount = secret.Value;

            }
            /* If you have throttling errors see this tutorial https://docs.microsoft.com/azure/key-vault/tutorial-net-create-vault-azure-web-app */
            /// <exception cref="KeyVaultErrorException">
            /// Thrown when the operation returned an invalid status code
            /// </exception>
            catch (KeyVaultErrorException keyVaultException)
            {
                PAT = keyVaultException.Message;
            }
        }

    }
}
