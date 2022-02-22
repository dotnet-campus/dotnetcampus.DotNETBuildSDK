using System;
using System.IO;

namespace dotnetCampus.Comparison
{
    /// <summary>
    /// 两个 Opc 文件不匹配异常
    /// </summary>
    public class OpenPackageConventionFileNoMatchException : Exception
    {
        /// <summary>
        /// 两个 Opc 文件不匹配异常
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="xmlFileName"></param>
        /// <param name="opcFile1"></param>
        /// <param name="opcFile2"></param>
        /// <param name="xmlFile1"></param>
        /// <param name="xmlFile2"></param>
        public OpenPackageConventionFileNoMatchException(ElementNotMatchException exception, string xmlFileName,
            FileInfo opcFile1, FileInfo opcFile2, FileInfo xmlFile1, FileInfo xmlFile2) : base(ToString(xmlFileName, exception, opcFile1, opcFile2), exception)
        {
            Exception = exception;
            XmlFileName = xmlFileName;
            OpcFile1 = opcFile1;
            OpcFile2 = opcFile2;
            XmlFile1 = xmlFile1;
            XmlFile2 = xmlFile2;
        }

        /// <summary>
        /// 元素不匹配异常
        /// </summary>
        public ElementNotMatchException Exception { get; }

        /// <summary>
        /// 不匹配的文件
        /// </summary>
        public string XmlFileName { get; }

        /// <summary>
        /// 对比的 OPC 文件
        /// </summary>
        public FileInfo OpcFile1 { get; }

        /// <summary>
        /// 对比的 OPC 文件
        /// </summary>
        public FileInfo OpcFile2 { get; }

        /// <summary>
        /// 对比的 XML 文件
        /// </summary>
        public FileInfo XmlFile1 { get; }

        /// <summary>
        /// 对比的 XML 文件
        /// </summary>
        public FileInfo XmlFile2 { get; }

        /// <summary>
        /// 不匹配元素所在文档的行数
        /// </summary>
        public int LineNumber => Exception.LineNumber;

        /// <inheritdoc />
        public override string ToString()
        {
            var xmlFileName = XmlFileName;
            var exception = Exception;

            var opcFile1 = OpcFile1;
            var opcFile2 = OpcFile2;

            return ToString(xmlFileName, exception, opcFile1, opcFile2);
        }

        private static string ToString(string xmlFileName, ElementNotMatchException exception, FileInfo opcFile1, FileInfo opcFile2)
        {
            return $"{xmlFileName} 文件不匹配。不匹配的行是第 {exception.LineNumber} 行。不匹配的元素是 {exception.Element1.Name} ;内容分别是 {exception.Element1.Value} 和 {exception.Element2?.Value} ;\r\n文件1:{opcFile1.FullName} ;\r\n文件2:{opcFile2} ;";
        }
    }
}