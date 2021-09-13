using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace dotnetCampus.Comparison.Tests
{
    [TestClass]
    public class XmlComparerTest
    {
        [ContractTestCase]
        public void IgnoreElement()
        {
            "加上忽略的元素列表，可以忽略此元素的不同，没有抛出不匹配异常".Test(() =>
            {
                var xmlString1 = @"<Foo>
<Id>123123</Id>
<F2>1.123</F2>
<F2>2.123</F2>
<F2>3.123</F2>
<F2>4.123</F2>
<F2>5.123</F2>
<F2>6.123</F2>
</Foo>";
                var xDocument1 = XDocument.Parse(xmlString1);
                var xmlString2 = @"<Foo>
<Id>123</Id>
<F2>1.123</F2>
<F2>2.123</F2>
<F2>3.123</F2>
<F2>4.123</F2>
<F2>5.123</F2>
<F2>6.123</F2>
</Foo>";
                var xDocument2 = XDocument.Parse(xmlString2);

                XmlComparer.VerifyXmlEquals(xDocument1, xDocument2, new XmlComparerSettings()
                {
                    IgnoreElementNameList = new [] { "Id" }
                });
            });
        }

        [ContractTestCase]
        public void VerifyXmlEquals()
        {
            "传入有相同列表元素的 XML 内容，没有抛出不匹配异常".Test(() =>
            {
                var xmlString = @"<Foo>
<F2>1.123</F2>
<F2>2.123</F2>
<F2>3.123</F2>
<F2>4.123</F2>
<F2>5.123</F2>
<F2>6.123</F2>
</Foo>";

                var xmlString1 = xmlString;
                var xDocument1 = XDocument.Parse(xmlString1);
                var xmlString2 = xmlString;
                var xDocument2 = XDocument.Parse(xmlString2);

                XmlComparer.VerifyXmlEquals(xDocument1, xDocument2);
            });

            "传入一个嵌套两层和一个嵌套一层的 XML 内容，抛出异常".Test(() =>
            {
                var xmlString1 = @"<Foo><F2>1.123</F2></Foo>";
                var xDocument1 = XDocument.Parse(xmlString1);
                var xmlString2 = @"<Foo>1.12301</Foo>";
                var xDocument2 = XDocument.Parse(xmlString2);

                Assert.ThrowsException<ElementNoMatchException>(() =>
                {
                    XmlComparer.VerifyXmlEquals(xDocument1, xDocument2);
                });
            });

            "传入嵌套多层的两个相同的 XML 内容，没有抛出不匹配异常".Test(() =>
            {
                var xmlString = @"<Foo><F2>1.123</F2></Foo>";
                var xmlString1 = xmlString;
                var xDocument1 = XDocument.Parse(xmlString1);
                var xmlString2 = xmlString;
                var xDocument2 = XDocument.Parse(xmlString2);

                XmlComparer.VerifyXmlEquals(xDocument1, xDocument2);
            });

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
