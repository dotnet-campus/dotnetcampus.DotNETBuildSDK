using System;
using System.Xml;
using System.Xml.Linq;

namespace dotnetCampus.Comparison
{
    /// <summary>
    /// 元素不匹配异常
    /// </summary>
    public class ElementNotMatchException : ArgumentException
    {
        /// <summary>
        /// 创建元素不匹配异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="element1"></param>
        /// <param name="element2"></param>
        public ElementNotMatchException(string? message, XElement element1, XElement? element2) : base(message)
        {
            Element1 = element1 ?? throw new ArgumentNullException(nameof(element1));
            Element2 = element2;

            if (element1 is IXmlLineInfo xmlLineInfo)
            {
                LineNumber = xmlLineInfo.LineNumber;
            }
        }

        /// <summary>
        /// 第一个 XML 元素
        /// </summary>
        public XElement Element1 { get; }

        /// <summary>
        /// 第二个 XML 元素
        /// </summary>
        public XElement? Element2 { get; }

        /// <summary>
        /// 不匹配元素所在文档的行数
        /// </summary>
        public int LineNumber { get; }

        /// <inheritdoc />
        public override string Message => $"ElementName:{Element1.Name};\r\nLineNumber:{LineNumber};\r\nValue1:{Element1.Value};\r\nValue2:{Element2?.Value}";
    }
}