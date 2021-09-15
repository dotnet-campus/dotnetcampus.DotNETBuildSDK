using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace dotnetCampus.Comparison
{
    /// <summary>
    /// 对比 XML 的工具
    /// </summary>
    public static class XmlComparer
    {
        /// <summary>
        /// 判断 XML 两个文件是否相等
        /// </summary>
        /// <exception cref="ElementNoMatchException"></exception>
        public static void VerifyXmlEquals(FileInfo file1, FileInfo file2, XmlComparerSettings? settings = null)
        {
            using var fileStream = file1.OpenRead();
            using var fileStream2 = file2.OpenRead();

            var xDocument1 = XDocument.Load(fileStream, LoadOptions.SetLineInfo);
            var xDocument2 = XDocument.Load(fileStream2, LoadOptions.SetLineInfo);

            VerifyXmlEquals(xDocument1, xDocument2, settings);
        }

        /// <summary>
        /// 判断 XML 两个文档是否相等
        /// </summary>
        public static void VerifyXmlEquals(XDocument xDocument1, XDocument xDocument2,
            XmlComparerSettings? settings = null)
        {
            var xDocument1Root = xDocument1.Root;
            var xDocument2Root = xDocument2.Root;

            if (xDocument1Root is null && xDocument2Root is null)
            {
                return;
            }
            else if (xDocument1Root is null || xDocument2Root is null)
            {
                throw new ArgumentException($"存在XML文档没有内容");
            }

            VerifyXmlEquals(xDocument1Root, xDocument2Root, settings);
        }

        /// <summary>
        /// 判断 XML 两个元素是否相等
        /// </summary>
        public static void VerifyXmlEquals(XElement? xElement1, XElement? xElement2,
            XmlComparerSettings? settings = null)
        {
            if (xElement1 is null && xElement2 is null)
            {
                return;
            }
            else if (xElement1 is null || xElement2 is null)
            {
                throw new ArgumentException($"存在元素没有内容");
            }

            settings ??= new XmlComparerSettings();

            VerifyElementEquals(xElement1, xElement2, settings);
        }

        private static void VerifyElementEquals(XElement xElement1, XElement xElement2, XmlComparerSettings settings)
        {
            if (!string.Equals(xElement1.Name.LocalName, xElement2.Name.LocalName))
            {
                throw new ElementNoMatchException(
                    $"元素名不同。xElement1.Name={xElement1.Name.LocalName} ; xElement2.Name={xElement2.Name.LocalName}",
                    xElement1, xElement2);
            }

            if (CanIgnore(xElement1, xElement2, settings))
            {
                // 这是忽略的元素
                return;
            }

            if (xElement1.HasElements == xElement2.HasElements)
            {
                if (xElement1.HasElements)
                {
                    var elementCountDictionary = new Dictionary<XName, int>();

                    VerifyElementEquals(xElement1, xElement2, settings, elementCountDictionary);
                }
                else
                {
                    var value1 = xElement1.Value;
                    var value2 = xElement2.Value;

                    if (double.TryParse(value1, out var n1) && double.TryParse(value2, out var n2))
                    {
                        if (Math.Abs(n1 - n2) > 0.001)
                        {
                            Throw();
                        }
                    }
                    else
                    {
                        if (!string.Equals(value1, value2))
                        {
                            Throw();
                        }
                    }
                }
            }
            else
            {
                Throw($"元素包含的子元素数量不同");
            }

            void Throw(string? message=null)
            {
                throw new ElementNoMatchException(message, xElement1, xElement2);
            }
        }

        private static void VerifyElementEquals(XElement xElement1, XElement xElement2, XmlComparerSettings settings,
            Dictionary<XName, int> elementCountDictionary)
        {
            foreach (var subElement1 in xElement1.Elements())
            {
                if (CanIgnore(subElement1, null, settings))
                {
                    continue;
                }

                XElement subElement2;

                // 要求列表的元素顺序是相同的
                var subElement2List = xElement2.Elements(subElement1.Name).ToList();
                if (subElement2List.Count == 1)
                {
                    subElement2 = subElement2List[0];
                }
                else
                {
                    if (!elementCountDictionary.TryGetValue(subElement1.Name, out var count))
                    {
                        count = 0;
                    }

                    if (count >= subElement2List.Count)
                    {
                        throw new ElementNoMatchException(
                            $"元素包含的子元素数量不同。xElement1.Count={count} ; xElement2.Count={subElement2List.Count}",
                            subElement1, null);
                    }

                    subElement2 = subElement2List[count];

                    count++;
                    elementCountDictionary[subElement1.Name] = count;
                }

                VerifyElementEquals(subElement1, subElement2, settings);
            }
        }

        private static bool CanIgnore(XElement xElement1, XElement? xElement2, XmlComparerSettings settings)
        {
            if (settings.CanElementIgnore is not null)
            {
                if (settings.CanElementIgnore(xElement1))
                {
                    return true;
                }

                if (xElement2 is not null)
                {
                    return settings.CanElementIgnore(xElement2);
                }

                return false;
            }

            if (settings.IgnoreElementNameList is not null)
            {
                return settings.IgnoreElementNameList.Any(t => string.Equals(t, xElement1.Name.LocalName));
            }

            return false;
        }
    }
}