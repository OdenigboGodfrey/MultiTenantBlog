namespace MultiTenantBlogTest.src.Shared.Utilities
{
    public static class Utility
    {
        public static string prepareSubdomainName(string schemaName) {
            if (schemaName.Contains('-')) {
                schemaName = schemaName.Replace('-', '_');
            }
            return schemaName;
        }
    }
}