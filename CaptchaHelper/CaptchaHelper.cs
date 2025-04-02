using System.Text;
using SkiaSharp;

namespace Utils_DotNet.CaptchaHelper
{
    /// <summary>
    /// 使用SkiaSharp库的验证码生产类
    /// </summary>
    public class CaptchaHelper
    {
        /// <summary>
        /// 获取Tuple；
        /// Tuple 的第一个值是表达式结果，string；
        /// Tuple 的第二个值是表达式图片, byte[]；
        /// </summary>
        /// <returns></returns>
        public static Tuple<string, byte[]> GetImgData()
        {
            Tuple<string, string> tuple = GetCaptchaCode();
            var result = tuple.Item2.ToString();
            var img = CreateCaptchaImage(tuple.Item1);
            return new Tuple<string, byte[]>(result, img);
        }

        #region 获取验证码表达式和结果
        /// <summary>
        /// 获取一个包含验证码表达式和其结果值的元组
        /// </summary>
        /// <returns></returns>
        private static Tuple<string, string> GetCaptchaCode()
        {
            int result = 0;
            char[] operators = { '+', '-', '*' };
            StringBuilder expression = new();

            Random random = new Random();
            int firstNum = random.Next(10);
            int secondNum = random.Next(10);
            char operatorChar = operators[random.Next(operators.Length)];

            switch (operatorChar)
            {
                case '+':
                    result = firstNum + secondNum;
                    break;
                case '*':
                    result = firstNum * secondNum;
                    break;
                case '-':
                    if (firstNum < secondNum)
                    {
                        int temp = firstNum;
                        firstNum = secondNum;
                        secondNum = temp;
                    }
                    result = firstNum - secondNum;
                    break;
            }

            expression.Append((char)('0' + firstNum))
                .Append(operatorChar)
                .Append((char)('0' + secondNum))
                .Append("=?");

            return new Tuple<string, string>(expression.ToString(), result.ToString());
        }
        #endregion 获取验证码表达式和结果

        #region 生成验证码图片
        /// <summary>
        /// 根据表达式字符串生成对应图片
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static byte[] CreateCaptchaImage(String expression)
        {
            Random random = new Random();
            // Color
            var colors = new[] { SKColors.Red, SKColors.Green, SKColors.DarkBlue, SKColors.Black, SKColors.Orange, SKColors.Brown, SKColors.DarkCyan, SKColors.Purple };
            // Font
            var fonts = new[] { "Consoles", "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            // Canvas Configuration
            using var image2d = new SKBitmap(100, 30, SKColorType.Srgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(image2d);
            canvas.DrawColor(SKColors.AntiqueWhite);
            using var drawStyle = new SKPaint();
            using var drawFont = new SKFont();
            // 填充验证码到图片
            for (int i = 0; i < expression.Length; i++)
            {
                drawStyle.IsAntialias = true;
                drawFont.Size = 30;
                drawFont.Typeface = SKTypeface.FromFamilyName(fonts[random.Next(fonts.Length)], SKFontStyleWeight.SemiBold, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright);
                drawStyle.Color = colors[random.Next(colors.Length)];
                string text = expression[i].ToString();
                int width = 16 * (i + 1);
                int height = 28;
                canvas.DrawText(text, width, height, SKTextAlign.Center, drawFont, drawStyle);

            }
            // 生成干扰线
            for (int i = 0; i <= expression.Length; i++)
            {
                drawStyle.Color = colors[random.Next(colors.Length)];
                drawStyle.StrokeWidth = 1;
                canvas.DrawLine(random.Next(0, expression.Length * 15), random.Next(0, 60), random.Next(0, expression.Length * 16), random.Next(0, 30), drawStyle);
            }
            //创建对象信息
            using var img = SKImage.FromBitmap(image2d);
            using var pic = img.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            //保存到流
            pic.SaveTo(stream);
            var captchaBytes = stream.GetBuffer();
            return captchaBytes;
        }

        #endregion 生成验证码图片

    }
}
