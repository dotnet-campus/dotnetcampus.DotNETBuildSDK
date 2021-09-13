using System;
using System.Xml;
using System.Xml.Linq;

namespace dotnetCampus.Comparison
{
    /// <summary>
    /// 元素不匹配异常
    /// </summary>
    public class ElementNoMatchException : ArgumentException
    {
        /// <summary>
        /// 创建元素不匹配异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="element1"></param>
        /// <param name="element2"></param>
        public ElementNoMatchException(string message, XElement element1, XElement? element2) : base(message)
        {
            Element1 = element1;
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
        /// 行数
        /// </summary>
        public int LineNumber { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"ElementName:{Element1.Name};\r\nLineNumber:{LineNumber};\r\n{base.ToString()}";
        }
    }
}