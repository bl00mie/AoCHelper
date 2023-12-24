using System.Net;

namespace AoCHelper
{
    /// <summary>
    /// <see cref="BaseProblem"/> with custom <see cref="BaseProblem.ClassPrefix"/> ("Day")
    /// </summary>
    public abstract class BaseDay(int year) : BaseProblem
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override string ClassPrefix { get; } = "Day";

        public override string InputFilePath
            => Path.Combine(InputFileDirPath, $"{Year}_{Day:D2}");

        protected virtual int Year { get; set; } = year;

        private int _day;
        protected virtual int Day
        {
            get
            {
                if (_day == default) _day = (int)CalculateIndex();
                return _day;
            }
            set
            {
                _day = value;
            }
        }

        protected string[] Input = [];

        private async Task<IEnumerable<string>> GetInput()
        {
            if (!File.Exists(InputFilePath))
            {
                if (!File.Exists("../session_cookie"))
                    throw new FileNotFoundException("Couldn't locate session_cookie file. Aborting");
                var sessionCookie = File.ReadLines("../session_cookie").First();

                var baseAddress = new Uri($"https://adventofcode.com/{Year}/day/{Day}/input");
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

        public override async Task InitializeInputAsync()
        {
            Input = (await GetInput()).ToArray();
        }

        public override void ProcessInput() { }
    }
}
