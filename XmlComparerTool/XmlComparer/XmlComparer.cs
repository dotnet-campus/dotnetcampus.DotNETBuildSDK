using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace dotnetCampus.Comparison
{
    public static class XmlComparer
    {
        /// <summary>
        /// 判断 XML 两个文件是否相等
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <exception cref="ElementNoMatchException"></exception>
        public static void VerifyXmlEquals(FileInfo file1, FileInfo file2)
        {
            using var fileStream = file1.OpenRead();
            using var fileStream2 = file2.OpenRead();

            var xDocument1 = XDocument.Load(fileStream);
            var xDocument2 = XDocument.Load(fileStream2);

            VerifyXmlEquals(xDocument1, xDocument2);
        }

        /// <summary>
        /// 判断 XML 两个文档是否相等
        /// </summary>
        public static void VerifyXmlEquals(XDocument xDocument1, XDocument xDocument2)
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

            VerifyXmlEquals(xDocument1Root, xDocument2Root);
        }

        /// <summary>
        /// 判断 XML 两个元素是否相等
        /// </summary>
        public static void VerifyXmlEquals(XElement xElement1, XElement xElement2)
        {
            if (xElement1.HasElements == xElement2.HasElements)
            {
                if (xElement1.HasElements)
                {
                    var elementCount = new Dictionary<XName, int>();

                    foreach (var subElement1 in xElement1.Elements())
                    {
                        XElement subElement2;

                        // 要求列表的元素顺序是相同的
                        var subElement2List = xElement2.Elements(subElement1.Name).ToList();
                        if (subElement2List.Count == 1)
                        {
                            subElement2 = subElement2List[0];
                        }
                        else
                        {
                            if (!elementCount.TryGetValue(subElement1.Name, out var count))
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
                            elementCount[subElement1.Name] = count;
                        }

                        VerifyXmlEquals(subElement1, subElement2);
                    }
                }
                else
                {
                    var value1 = xElement1.Value;
                    var value2 = xElement2.Value;

                    if (double.TryParse(value1, out var n1) && double.TryParse(value2, out var n2))
                    {
                        if (Math.Abs(n1 - n2) > 0.001)
                        {
                            Throw($"元素的值不匹配，分别是 {value1} 和 {value2}");
                        }
                    }
                    else
                    {
                        if (!string.Equals(value1, value2))
                        {
                            Throw($"元素的值不匹配，分别是 {value1} 和 {value2}");
                        }
                    }
                }
            }
            else
            {
                Throw($"元素包含的子元素数量不同");
            }

            void Throw(string message)
            {
                throw new ElementNoMatchException(message, xElement1, xElement2);
            }
        }
    }
}