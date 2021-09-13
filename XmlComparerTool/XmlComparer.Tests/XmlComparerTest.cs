using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace dotnetCampus.Comparison.Tests
{
    [TestClass]
    public class XmlComparerTest
    {
        [ContractTestCase]
        public void VerifyXmlEquals()
        {
            "传入浮点值，存在精度误差，没有抛出不匹配异常".Test(() =>
            {
                var xmlString1 = @"<Foo>1.123</Foo>";
                var xDocument1 = XDocument.Parse(xmlString1);
                var xmlString2 = @"<Foo>1.12301</Foo>";
                var xDocument2 = XDocument.Parse(xmlString2);

                XmlComparer.VerifyXmlEquals(xDocument1, xDocument2);
            });

            "传入值不相同的XML内容，抛出异常".Test(() =>
            {
                var xmlString1 = @"<Foo>Foo</Foo>";
                var xDocument1 = XDocument.Parse(xmlString1);
                var xmlString2 = @"<Foo>F1</Foo>";
                var xDocument2 = XDocument.Parse(xmlString2);

                Assert.ThrowsException<ElementNoMatchException>(() =>
                {
                    XmlComparer.VerifyXmlEquals(xDocument1, xDocument2);
                });
            });

            "传入两个完全相同的XML内容，没有抛出不匹配异常".Test(() =>
            {
                var xmlString = @"<Foo>Foo</Foo>";
                var xDocument1 = XDocument.Parse(xmlString);
                var xDocument2 = XDocument.Parse(xmlString);

                XmlComparer.VerifyXmlEquals(xDocument1, xDocument2);
            });
        }
    }
}
