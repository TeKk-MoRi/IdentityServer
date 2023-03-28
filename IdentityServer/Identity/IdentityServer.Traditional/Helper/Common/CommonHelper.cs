using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using IdentityServer.Traditional.Helper.Enums;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Traditional.Helper.Common
{
    public static class CommonHelper
    {
        private const char YehArabic = 'ي';
        private const char YehPersian = 'ی';
        private const char KafArabic = 'ك';
        private const char KafPersian = 'ک';

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            email = email.Trim();

            var result = Regex.IsMatch(email, RegularExpressionConstants.Email, RegexOptions.IgnoreCase);

            return result;
        }

        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);

                if (!string.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }

                return result;
            }

            return str;
        }

        public static string EnsureNumericOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var result = new StringBuilder();

            foreach (char c in str)
            {
                if (char.IsDigit(c))
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static IEnumerable<string> ReadTextFileLines(Stream file)
        {
            using (var reader = new StreamReader(file))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static IEnumerable<string[]> ReadTextFileLines(Stream file, char wordSeparator)
        {
            using (var reader = new StreamReader(file))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        yield return line.Split(wordSeparator);
                    }
                }
            }
        }

        public static string GetConnectionSetting(string key)
        {
            return ConfigurationManager.GetConnectionString(key);
        }

        public static string GetConfigurationSetting(string key)
        {
            return ConfigurationManager.GetValue<string>(key);
        }
        public static T GetConfigurationSetting<T>(string key)
        {
            return ConfigurationManager.GetValue<T>(key);
        }

        public static async Task<string> GetWebRequest(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                return await response.Content.ReadAsStringAsync();
            }
        }

        public static string SerializeJson<T>(this T value, bool enableCamelCase = true)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return CommonHelper.Serialize(value, enableCamelCase);
        }

        public static string SerializeXml<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var xmlserializer = new XmlSerializer(typeof(T));

            var stringWriter = new StringWriter();

            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, value);

                return stringWriter.ToString();
            }
        }

        public static string SerializeXml<T>(this T value, string rootName)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var xmlserializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            var stringWriter = new StringWriter();

            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, value);

                return stringWriter.ToString();
            }
        }

        public static string Serialize(object obj, bool enableCamelCase = true)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (enableCamelCase)
            {
                return JsonConvert.SerializeObject(obj,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
            }

            return JsonConvert.SerializeObject(obj);
        }

        public static T ToObject<T>(object obj)
        {
            return obj == null ? default(T) : Deserialize<T>(Serialize(obj));
        }

        public static T Deserialize<T>(string serializedValue)
        {
            return string.IsNullOrEmpty(serializedValue)
                ? default(T)
                : JsonConvert.DeserializeObject<T>(serializedValue);
        }

        public static T Deserialize<T>(string serializedValue, Type type)
        {
            return string.IsNullOrEmpty(serializedValue)
                ? default(T)
                : (T)JsonConvert.DeserializeObject(serializedValue, type);
        }

        public static string GetBase64StringFromDataUrl(string dataUrl)
        {
            return dataUrl.Split(',')[1];
        }

        public static byte[] GetBase64BufferFromDataUrl(string dataUrl)
        {
            return GetBase64BufferFromBase64String(GetBase64StringFromDataUrl(dataUrl));
        }

        public static byte[] GetBase64BufferFromBase64String(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        public static IEnumerable<byte[]> GetBase64BufferFromDataUrl(IEnumerable<string> dataUrls)
        {
            return GetBase64BufferFromDataUrl(dataUrls.ToArray());
        }

        public static IEnumerable<byte[]> GetBase64BufferFromDataUrl(params string[] dataUrls)
        {
            foreach (var dataUrl in dataUrls)
            {
                yield return GetBase64BufferFromBase64String(GetBase64StringFromDataUrl(dataUrl));
            }
        }

        public static string ConvertToEnglishNumber(string input)
        {
            string[] persianNumbers = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int index = 0; index < persianNumbers.Length; index++)
            {
                input = input.Replace(persianNumbers[index], index.ToString());
            }

            return input;
        }

        public static string ConvertToPersianKeAndYe(this string input)
        {
            return input?.Replace(KafArabic, KafPersian).Replace(YehArabic, YehPersian);
        }

        public static string ConvertToPersianNumber(this string input)
        {
            string[] persianNumbers = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int index = 0; index < persianNumbers.Length; index++)
            {
                input = input.Replace(index.ToString(), persianNumbers[index]);
            }

            return input;
        }

        public static string ConvertToPersianNumber(this int input)
        {
            return ConvertToPersianNumber(input.ToString());
        }

        public static string DecodeHtml(string value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        public static string EncodeUrl(string value)
        {
            //return Microsoft.Security.Application.Encoder.UrlEncode(value);
            return HttpUtility.UrlEncode(value);
        }

        public static string DecodeUrl(string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        public static NameValueCollection ParseQueryString(string value)
        {
            return HttpUtility.ParseQueryString(value);
        }

        public static string ToggleSlashAtStart(this string value, bool shouldHave)
        {
            return ToggleStringAtStart(value, "/", shouldHave);
        }

        public static string ToggleSlashAtEnd(this string value, bool shouldHave)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (shouldHave)
            {
                if (!value.EndsWith("/"))
                {
                    value += "/";
                }
            }
            else
            {
                if (value.EndsWith("/"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }

            return value;
        }

        public static string ToggleBackSlashAtStart(this string value, bool shouldHave)
        {
            return ToggleStringAtStart(value, "\\", shouldHave);
        }

        public static string ToggleBackSlashAtEnd(this string value, bool shouldHave)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (shouldHave)
            {
                if (!value.EndsWith("\\"))
                {
                    value += "\\";
                }
            }
            else
            {
                if (value.EndsWith("\\"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }

            return value;
        }

        public static string ToggleStringAtStart(this string value, string stringValue, bool shouldHave)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (shouldHave)
            {
                if (!value.StartsWith(stringValue))
                {
                    value = stringValue + value;
                }
            }
            else
            {
                if (value.StartsWith(stringValue))
                {

                    value = value.Substring(stringValue.Length, value.Length - stringValue.Length);
                }
            }

            return value;
        }

        public static string ToggleStringAtEnd(this string value, string stringValue, bool shouldHave)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (shouldHave)
            {
                if (!value.EndsWith(stringValue))
                {
                    value += stringValue;
                }
            }
            else
            {
                if (value.EndsWith(stringValue))
                {
                    value = value.Substring(0, value.Length - stringValue.Length);
                }
            }

            return value;
        }

        public static string GetCorrectUserIpAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                throw new ArgumentNullException(nameof(ip));
            }

            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }

            //  Check if IP Address has Port number
            int portIndex = ip.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);

            if (portIndex > 0)
            {
                ip = ip.Substring(0, portIndex);
            }

            return ip;
        }

        public static string GenerateSlug(this string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }

            var slug = title;

            slug = Regex.Replace(slug,
                @"[^A-Za-z0-9\u0627-\u0648\uFB8A\u067E\u0686\u06AF\u0698\u06A9\u06AF\u06CC\u06F0-\u06F9\s-]", "");
            slug = Regex.Replace(slug, @"[\s-]+", " ").Trim();
            slug = slug.Substring(0, slug.Length).Trim();
            slug = Regex.Replace(slug, @"\s", "-");

            return slug.ToLower();
        }

        public static bool CanConvertToLong(object value)
        {
            try
            {
                Convert.ToInt64(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CanConvertToGuid(object value)
        {
            try
            {
                Guid.Parse(value.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPasswordStrength(string password)
        {
            return Regex.IsMatch(password, RegularExpressionConstants.PasswordStrength);
        }

        public static bool IsPersianDateValid(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            //  Make sure all numbers are english
            string englishNumbers = ConvertToEnglishNumber(value);

            return Regex.IsMatch(englishNumbers, RegularExpressionConstants.PersianDate);
        }

        public static byte[] ToArray(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var totalBuffers = new List<byte>();

            const int bufferSize = 0x1000;

            var buffer = new byte[bufferSize];

            int readCount;

            while ((readCount = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                totalBuffers.AddRange(buffer.Take(readCount));
            }

            return totalBuffers.ToArray();
        }

        public static string GenerateNewFileName()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GenerateUniqCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string GenerateNewFileNameWithExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            //  Remove . from extension
            extension = extension.Replace(".", "");

            return $"{GenerateNewFileName()}.{extension}";
        }

        public static string GetFileName(string fileName)
        {
            return Path.GetFileName(fileName);
        }

        public static string ChangeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            fileName = fileName.ToggleStringAtEnd("-", true);

            return fileName + GenerateNewFileName();
        }

        public static bool IsFileLocked(string filePath)
        {
            FileStream stream = null;

            try
            {
                stream = new FileInfo(filePath).Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked

            return false;
        }

        public static bool HasDuplicate<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(a => a).SelectMany(grp => grp.Skip(1)).Any();
        }

        public static bool HasDuplicate<T, TKey>(this IEnumerable<T> values, Func<T, TKey> constraint)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            return values.GroupBy(constraint).Count() != values.Count();
        }

        public static bool IsPassportNumberValid(string passportNumber)
        {
            return Regex.IsMatch(passportNumber, RegularExpressionConstants.PassportNumber);
        }

        public static int CalculateAge(DateTime birthdayDate)
        {
            var today = DateTime.Today;

            int age = today.Year - birthdayDate.Year;

            if (birthdayDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        public static bool IsPersianText(string text)
        {
            return Regex.IsMatch(text, RegularExpressionConstants.PersianAlphanumeric);
        }

        public static bool IsLatinText(string text)
        {
            return Regex.IsMatch(text, RegularExpressionConstants.EnglishAlphabet);
        }

        public static string GetPicPathBySize(string imageUrl, int width, int height)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return string.Empty;
            }

            if (width == 0)
            {
                return
                    $"{Path.GetDirectoryName(imageUrl).Replace('\\', '/')}/{Path.GetFileNameWithoutExtension(imageUrl)}{Path.GetExtension(imageUrl)}?h={height}";
            }

            if (height == 0)
            {
                return
                    $"{Path.GetDirectoryName(imageUrl).Replace('\\', '/')}/{Path.GetFileNameWithoutExtension(imageUrl)}{Path.GetExtension(imageUrl)}?w={width}";
            }

            return
                $"{Path.GetDirectoryName(imageUrl).Replace('\\', '/')}/{Path.GetFileNameWithoutExtension(imageUrl)}-{width}x{height}{Path.GetExtension(imageUrl)}";
        }

        public static string EnumValue(Type type, string name)
        {
            var counter = 0;

            foreach (var enumName in System.Enum.GetNames(type))
            {
                if (enumName.ToLower() == name.ToLower())
                {
                    try
                    {
                        var subCounter = 0;

                        foreach (var value in System.Enum.GetValues(type))
                        {
                            if (subCounter++ == counter)
                            {
                                return Convert.ToInt32(value).ToString();
                            }
                        }
                    }
                    catch
                    {
                        return counter.ToString();
                    }
                }

                counter++;
            }

            return null;
        }

        public static string GetParameter(string parameters, string parameter, char seperator = '/')
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return string.Empty;
            }

            var urlDecode = HttpUtility.UrlDecode(parameters);

            if (urlDecode != null)
            {
                var value =
                    urlDecode.Split(seperator).FirstOrDefault(r => r.ToLower().StartsWith(parameter.ToLower() + "-"));

                if (string.IsNullOrWhiteSpace(value))
                {
                    return string.Empty;
                }

                return value.ToLower().Replace(parameter.ToLower() + "-", "");
            }

            return string.Empty;
        }

        public static string GetParameterWithSlashFormat(string parameters, string parameter)
        {
            var seperator = '/';

            if (string.IsNullOrWhiteSpace(parameters))
            {
                return string.Empty;
            }

            var urlDecode = HttpUtility.UrlDecode(parameters);

            if (urlDecode != null)
            {
                var indexOf = urlDecode.IndexOf(parameter + seperator, StringComparison.Ordinal);

                if (indexOf > -1)
                {
                    var endIndexOf = urlDecode.IndexOf(seperator.ToString(), indexOf + (parameter + seperator).Length,
                        StringComparison.Ordinal);

                    if (endIndexOf > -1)
                    {
                        return urlDecode.Substring(indexOf + (parameter + seperator).Length,
                            endIndexOf - (indexOf + (parameter + seperator).Length));
                    }
                }
            }

            return string.Empty;
        }

        public static ImageFormats GetImageFormat(string imageFileName)
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                throw new Exception("image name is null or empty");
            }

            switch (Path.GetExtension(imageFileName.ToLower()))
            {
                case ".jpg":
                    return ImageFormats.Jpg;

                case ".jpeg":
                    return ImageFormats.Jpeg;

                case ".png":
                    return ImageFormats.Png;

                case ".tif":
                    return ImageFormats.Tif;

                case ".bmp":
                    return ImageFormats.Bmp;

                case ".gif":
                    return ImageFormats.Gif;

                default:
                    throw new Exception("this format not supported");
            }
        }

        public static string GetFormatFromBase64String(string base64)
        {
            var format = base64.Split(',');

            if (format.Length != 2)
            {
                throw new Exception(nameof(base64) + " is not base64string");
            }

            var extension = format[0].Split('/');

            if (extension.Length != 2)
            {
                throw new Exception(nameof(extension) + " is not base64string");
            }

            return "." + extension[1].Split(';')[0];
        }

        public static bool IsDevelopmentMode()
        {
            bool result = false;

#if DEBUG
            result = true;
#endif

            return result;
        }

        /// <summary>
        /// Unique string from time Ticks
        /// </summary>
        /// <returns>unique string value</returns>
        public static string GenerateUniqueString()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string unique = Convert.ToBase64String(bytes)
                .Replace('+', '_')
                .Replace('/', '-')
                .TrimEnd('=');
            return unique;
        }

        public static bool IsIranianLegalIdValid(string id)
        {
            //input has 11 digits that all of them are not equal
            if (!Regex.IsMatch(id, @"^(?!(\d)\1{10})\d{11}$"))
                return false;

            var check = Convert.ToInt32(id.Substring(10, 1));
            int dec = Convert.ToInt32(id.Substring(9, 1)) + 2;
            int[] Coef = new int[10] { 29, 27, 23, 19, 17, 29, 27, 23, 19, 17 };

            var sum = Enumerable.Range(0, 10)
                .Select(x => (Convert.ToInt32(id.Substring(x, 1)) + dec) * Coef[x])
                .Sum() % 11;
            sum = sum == 10 ? 0 : sum; // by 10101149480
            return sum == check;
        }

        public static bool IsIranianNationalIdValid(string id)
        {
            if (id?.Length != 10)
                return false;

            id = id.PadLeft(10, '0');

            if (!Regex.IsMatch(id, @"^\d{10}$"))
                return false;

            var check = Convert.ToInt32(id.Substring(9, 1));
            var sum = Enumerable.Range(0, 9)
                .Select(x => Convert.ToInt32(id.Substring(x, 1)) * (10 - x))
                .Sum() % 11;

            return sum < 2 && check == sum || sum >= 2 && check + sum == 11;
        }

        public static bool IsIranianMobileValid(string mobile)
        {
            return !string.IsNullOrWhiteSpace(mobile) && Regex.IsMatch(mobile, RegularExpressionConstants.IranianMobile);
        }

        public static bool IsIranianTelephoneValid(string telephone)
        {
            return !string.IsNullOrWhiteSpace(telephone) &&
                   Regex.IsMatch(telephone, RegularExpressionConstants.IranianTelephone);
        }

        public static bool IsShebaNumberValid(string number)
        {
            return !string.IsNullOrWhiteSpace(number) && Regex.IsMatch(number, RegularExpressionConstants.ShebaNumber);
        }

        public static bool IsNonEnglishWord(string word)
        {
            return Regex.IsMatch(word, @"[^\x00-\x7F]+");
        }

        public static string HashUsingSha512(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            using (var sha = System.Security.Cryptography.SHA512.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(text)));
            }
        }

        public static string HashUsingSha1(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            using (var sha = System.Security.Cryptography.SHA1.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(text)));
            }
        }

        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            var num = dateTime.ToUniversalTime().Ticks / 10000000;
            return (num - 62135596800);
        }

        public static long? ToUnixTimeSeconds(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;
            return ToUnixTimeSeconds(dateTime.Value);
        }

        public static DateTime FromUnixTimeSeconds(this long seconds, bool localTime)
        {
            if ((seconds < -62135596800) || (seconds > 253402300799))
            {
                throw new ArgumentOutOfRangeException(nameof(seconds),
                    $"ArgumentOutOfRange_Range {-62135596800L} {253402300799}");
            }
            var result = new DateTimeOffset((seconds * 0x989680L) + 0x89f7ff5f7b58000L, TimeSpan.Zero);

            return localTime ? result.LocalDateTime : result.UtcDateTime; ;

        }

        public static DateTime? FromUnixTimeSeconds(this long? seconds, bool localTime)
        {
            if (!seconds.HasValue)
                return null;

            return FromUnixTimeSeconds(seconds.Value, localTime);
        }

        public static bool ExistValue<TEnum>(string value)
        {
            var values = System.Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            return values.Any(r => Convert.ToInt32(r).ToString() == value);
        }

        public static string CleanNoneLatinCharacter(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);

            var chars = new[] { 1619, 1620 };

            var exceptionChars = chars.Select(x => (char)x).ToList();

            var stringBuilder = new StringBuilder();

            foreach (var character in normalizedString)
            {
                if (exceptionChars.Contains(character))
                {
                    stringBuilder.Append(character);
                    continue;
                }

                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool TryParseLong(string numeric)
        {
            try
            {
                Convert.ToInt64(numeric);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsImageExtention(this string extention)
        {
            var imageExtentions = new[] { ".jpg", ".jpeg", ".png" };

            return imageExtentions.Contains(extention);
        }

        public static void LoadProperties(object loadObject, Dictionary<string, string> propertiesValues)
        {
            foreach (var property in loadObject.GetType().GetProperties())
            {
                if (propertiesValues.ContainsKey(property.Name))
                {
                    if (!string.IsNullOrEmpty(propertiesValues[property.Name]))
                    {
                        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                        var tc = TypeDescriptor.GetConverter(underlyingType);

                        var value = tc.ConvertFromString(null, culture: CultureInfo.InvariantCulture, text: propertiesValues[property.Name]);

                        property.SetValue(loadObject, value);
                    }
                    else
                    {
                        //Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        //todo for null values no action?
                    }
                }

            }
        }

        public static Dictionary<string, string> GetPropertiesValus(object loadObject)
        {
            var dic = new Dictionary<string, string>();

            foreach (var property in loadObject.GetType().GetProperties())
            {
                dic.Add(
                    property.Name,
                    Convert.ToString(property.GetValue(loadObject),
                        CultureInfo.InvariantCulture
                    ));
            }

            return dic;
        }

        public static string GetPersianDayName(this byte day)
        {
            switch (day)
            {
                case 0:
                    return "شنبه";
                case 1:
                    return "یکشنبه";
                case 2:
                    return "دوشنبه";
                case 3:
                    return "سه شنبه";
                case 4:
                    return "چهارشنبه";
                case 5:
                    return "پنج شنبه";
                case 6:
                    return "جمعه";
                default:
                    return string.Empty;
            }
        }

        public static string CleanPersianLatin(this string input)
        {
            return input.CleanNoneLatinCharacter().ConvertToPersianKeAndYe().Trim();
        }

        public static bool IsEnglishContent(this string input)
        {
            var regex = new Regex("^[A-Za-z ]+$");

            return regex.IsMatch(input);
        }

        public static string RemoveUrlParameter(string url, string parameter)
        {
            var cleanUrl = Regex.Replace(url, parameter + "=([\\w]*)", "").Replace("?&", "?").Replace("&&", "&");

            if (cleanUrl.EndsWith("?"))
            {
                cleanUrl = cleanUrl.Remove(cleanUrl.Length - 1, 1);
            }
            if (cleanUrl.EndsWith("&"))
            {
                cleanUrl = cleanUrl.Remove(cleanUrl.Length - 1, 1);
            }

            return cleanUrl;
        }

        public static string Encrypt(string clearText, string encryptionKey)
        {
            try
            {
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    encryptor.Key = pdb.GetBytes(32);

                    encryptor.IV = pdb.GetBytes(16);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }

            return clearText;
        }

        public static string Decrypt(string cipherText, string encryptionKey)
        {
            try
            {
                if (cipherText == null)
                    return null;

                cipherText = cipherText.Replace(" ", "+");

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (var encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    encryptor.Key = pdb.GetBytes(32);

                    encryptor.IV = pdb.GetBytes(16);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }

                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }

            return cipherText;
        }

        public static void AddRange<T>(this ICollection<T> instance, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                instance.Add(item);
            }
        }

        public static string CleanEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            email = email.ToLowerInvariant().Trim();

            var emailParts = email.Split('@');

            var name = emailParts[0].Replace(".", string.Empty).Replace("+", string.Empty);

            var emailDomain = emailParts[1];

            string[] domainsAllowedDots =
            {
                "gmail.com",
                "facebook.com"
            };

            var isFromDomainsAllowedDots = domainsAllowedDots.Any(domain => emailDomain.Equals(domain));

            return !isFromDomainsAllowedDots ? email : $"{name}@{emailDomain}";
        }

        public static T GetValueFromDisplayName<T>(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == name)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == name)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentOutOfRangeException("name");
        }

        public static T Clone<T>(this T _object) where T : class
        {
            return Deserialize<T>(_object.SerializeJson());
        }

        public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> projection)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    yield break;
                }

                TSource previous = iterator.Current;

                while (iterator.MoveNext())
                {
                    yield return projection(previous, iterator.Current);

                    previous = iterator.Current;
                }
            }
        }

        public static string GenerateRandomString(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);

            var builder = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));
            }

            return builder.ToString();
        }

        public static string GenerateDigitNumber(int number = 7)
        {
            Random generator = new Random();

            return generator.Next(99999, 999999999).ToString().Substring(0, number);
        }

        public static string GenerateAccountNumber()
        {
            var date = DateTime.Now;

            var firstPart = date.Year.ToString().Substring(0, 2) + date.Month.ToString("00");

            var lastPart = date.Year.ToString().Substring(2) + date.Day.ToString("00");

            var digits = SplitInParts(GenerateDigitNumber(8), 4);

            var randomDigits = string.Join("-", digits);

            return $"{firstPart}-{randomDigits}-{lastPart}";
        }

        public static IEnumerable<String> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }
}
