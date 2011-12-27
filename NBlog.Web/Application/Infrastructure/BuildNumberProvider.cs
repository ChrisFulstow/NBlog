using System.Reflection;

namespace NBlog.Web.Application.Infrastructure
{
    /// <summary>
    /// Provides current build number
    /// </summary>
    public static class BuildNumberProvider
    {
        /// <summary>
        /// Returs
        /// </summary>
        /// <returns>A string containing Major, major rev, minor, minor rev and Revision</returns>
        public static string GetBuildNumber()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}