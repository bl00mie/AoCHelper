using System.Net;

namespace AoCHelper
{
    /// <summary>
    /// <see cref="BaseProblem"/> with custom <see cref="BaseProblem.ClassPrefix"/> ("Day")
    /// </summary>
    public abstract class BaseDay : BaseProblem
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override string ClassPrefix { get; } = "Day";

        public override string InputFilePath
            => Path.Combine(InputFileDirPath, $"{_year}_{_day:D2}");

        private int _year;
        private int _day;
        protected IEnumerable<string> Input = Enumerable.Empty<string>();

        protected async Task Initialize(int year, int day = 0)
        {
            _year = year;
            _day = day != 0 ? day : (int)CalculateIndex();
            Input = await GetInput();
        }

        private async Task<IEnumerable<string>> GetInput()
        {
            if (!File.Exists(InputFilePath))
            {
                if (!File.Exists("../session_cookie"))
                    throw new FileNotFoundException("Couldn't locate session_cookie file. Aborting");
                var sessionCookie = File.ReadLines("../session_cookie").First();

                var baseAddress = new Uri($"https://adventofcode.com/{_year}/day/{CalculateIndex()}/input");
                var cookieContainer = new CookieContainer();
                using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                using var client = new HttpClient(handler) { BaseAddress = baseAddress };
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                cookieContainer.Add(baseAddress, new Cookie("session", sessionCookie));
                var response = await client.GetAsync(baseAddress);
                if (response.IsSuccessStatusCode)
                {
                    if (!Directory.Exists(InputFileDirPath))
                        Directory.CreateDirectory(InputFileDirPath);
                    using var writer = new StreamWriter(InputFilePath);
                    writer.Write(await response.Content.ReadAsStringAsync());
                    writer.Flush();
                }
            }
            return File.ReadLines(InputFilePath);
        }

        protected static ValueTask<string> Answer<T>(T ans) => ValueTask.FromResult(ans?.ToString() ?? string.Empty);
    }
}
