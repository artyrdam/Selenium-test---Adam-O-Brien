using System;
using System.Diagnostics;
using System.Globalization;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CurrencyConverter_Selenium_Test
{
    public class TestClass
    {
        IWebDriver driver;
        
        string currencyFrom = "EUR";
        string currencyTo = "AUD";
        string conversionAmount = "100.50";
        
        [SetUp]
        public void OpenBrowser()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.xe.com/currencyconverter");
            System.Threading.Thread.Sleep(500);

        }


        [Test]
        public void test_Inputs()
        {  
            //Enter conversion details
            driver.FindElement(By.Id("amount")).SendKeys(conversionAmount);
            SelectDropDown("from",currencyFrom);
            SelectDropDown("to",currencyTo);
            
            //Assertions
            string amount = driver.FindElement(By.Id("amount")).GetProperty("value");
            Assert.AreEqual(amount,conversionAmount,"Currency value entered is not correct");

            
            string from = driver.FindElement(By.Name("From")).GetProperty("value");
            Assert.AreEqual(from,currencyFrom,"From currency chosen does not match entered type");
            
            string to = driver.FindElement(By.Name("To")).GetProperty("value");
            Assert.AreEqual(to,currencyTo,"To currency chosen does not match entered type");
        }

        [Test]
        public void test_Swap_Sources()
        {
            driver.FindElement(By.Id("amount")).SendKeys(conversionAmount);
            SelectDropDown("from",currencyFrom);
            SelectDropDown("to",currencyTo);
            
            string from = driver.FindElement(By.Name("From")).GetProperty("value");
            string to = driver.FindElement(By.Name("To")).GetProperty("value");
            
            //Click swap sources
            driver.FindElement(By.XPath("//*[@id='converterForm']/form/button[1]")).Click();
            
            string fromConv = driver.FindElement(By.Name("From")).GetProperty("value");
            string toConv = driver.FindElement(By.Name("To")).GetProperty("value");
            
            Assert.AreEqual(from,toConv,"From Swapped Currency is incorrect");
            Assert.AreEqual(to,fromConv,"To Swapped Currency is incorrect");
        }


        [Test]
        public void test_Conversion_Amounts()
        {
            //Enter conversion details
            driver.FindElement(By.Id("amount")).SendKeys(conversionAmount);
            SelectDropDown("from",currencyFrom);
            SelectDropDown("to",currencyTo);
            
            //Convert
            driver.FindElement(By.XPath("//*[@id='converterForm']/form/button[2]")).Click();
            System.Threading.Thread.Sleep(500);

            CompareConversionValues();

        }


        [Test]
        public void test_Negative_Values()
        {
            string negativeValue = "-" + conversionAmount;
            //Enter a conversion value
            driver.FindElement(By.Id("amount")).SendKeys(negativeValue);
            
            
            driver.FindElement(By.Id("amount")).SendKeys(Keys.Enter);
            System.Threading.Thread.Sleep(500);
            
            string amount = driver.FindElement(By.Id("amount")).GetAttribute("value");
            string displayedConversionAmount = driver.FindElement(By.XPath("//*[@id='converterResult']/div/div/div[1]/span[1]")).Text;
                
            //Assertions
            //Compares conversion amount entered with the conversion amounts displayed
            Assert.AreEqual(conversionAmount,amount,"Value entered has not been converted from a negative value to a positive");
            Assert.AreEqual(displayedConversionAmount,conversionAmount,"Incorrect currency amount has been used for conversion");
            
        }

        [Test]
        public void test_URI()
        {
            //Enter conversion details
            driver.FindElement(By.Id("amount")).SendKeys(conversionAmount);
            SelectDropDown("from",currencyFrom);
            SelectDropDown("to",currencyTo);
            
            //Click swap sources
            driver.FindElement(By.XPath("//*[@id='converterForm']/form/button[1]")).Click();
            //Convert
            driver.FindElement(By.XPath("//*[@id='converterForm']/form/button[2]")).Click();
            System.Threading.Thread.Sleep(500);
            
            //get entered data
            string amount = driver.FindElement(By.Id("amount")).GetAttribute("value");
            string currencyTypeFrom = driver.FindElement(By.Name("From")).GetAttribute("value");
            string currencyTypeTo = driver.FindElement(By.Name("To")).GetAttribute("value");
            
            //Get url data
            String currentURL = driver.Url;

            string[] UrlSplit = currentURL.Split('=','&');

            //Remove unnecessary characters
            foreach (var url in UrlSplit)
            {
                url.Trim(' ', '=', '&');
            }
            
            //assertions
            //compare entered value with uri
            Assert.AreEqual(amount,UrlSplit[1],"Entered currency value does not match URI");
            //compare To value with uri
            Assert.AreEqual(currencyTypeTo,UrlSplit[5],"Entered - To - value does not match URI");
            //compare From value with uri
            Assert.AreEqual(currencyTypeFrom,UrlSplit[3],"Entered - From - value does not match URI");
        }


        [Test]
        public void test_URI_Manipulation()
        {
            //Create URI
            string newURI = "https://www.xe.com/currencyconverter/convert/?Amount=" + conversionAmount + "&From=" + currencyFrom + "&To=" + currencyTo;
            
            driver.Navigate().GoToUrl(newURI);
            System.Threading.Thread.Sleep(500);
            
            //get entered data
            string amount = driver.FindElement(By.Id("amount")).GetAttribute("value");
            string currencyTypeFrom = driver.FindElement(By.Name("From")).GetAttribute("value");
            string currencyTypeTo = driver.FindElement(By.Name("To")).GetAttribute("value");
            
            //assertions
            //compare entered value with entered uri value
            Assert.AreEqual(amount,conversionAmount,"Entered currency value does not match URI");
            //compare To value with entered uri value
            Assert.AreEqual(currencyTypeTo,currencyTo,"Entered - To - value does not match URI");
            //compare From value with entered uri value
            Assert.AreEqual(currencyTypeFrom,currencyFrom,"Entered - From - value does not match URI");
        }


        [Test]
        public void test_Non_Numeric_Values()
        {
            //Randomize a letter
            char charRandom = RandomLetter();
            
            //Enter conversion details
            driver.FindElement(By.Id("amount")).SendKeys(charRandom.ToString());
            SelectDropDown("from",currencyFrom);
            SelectDropDown("to",currencyTo);
            
            //Convert
            driver.FindElement(By.XPath("//*[@id='converterForm']/form/button[2]")).Click();
            System.Threading.Thread.Sleep(500);

            //get entered data
            string amount = driver.FindElement(By.Id("amount")).GetAttribute("value");
            //Get displayed data
            string displayedConvValue = driver.FindElement(By.ClassName("kBpwaI")).Text;
            
            //Assertions
            //Check if value gets changed to 1 when entering non numeric value
            Assert.AreEqual(amount,"1","Conversion amount did not convert to 1 when entering non numeric value");
            //Compare conversion type to displayed type
            Assert.AreEqual(amount,displayedConvValue,"Currency type does not match chosen conversion currency");
        }


        [TearDown]
        public void CloseBrowser()
        {
            System.Threading.Thread.Sleep(5000);
            driver.Quit();
        }
        

        #region Methods


        public void SelectDropDown(string fromOrTo, string currencyType)
        {

            driver.FindElement(By.Id(fromOrTo)).SendKeys(currencyType);
            driver.FindElement(By.Id(fromOrTo)).SendKeys(Keys.Enter);

        }

        public void CompareConversionValues()
        {
            //Get converted values
            string amount = driver.FindElement(By.ClassName("converterresult-toAmount")).Text;
            string currencyType = driver.FindElement(By.ClassName("converterresult-toCurrency")).Text;
            string baseConversionAmount = driver.FindElement(By.XPath("//*[@id='converterResult']/section/div[1]/div[2]")).Text;
            
            //Strip value from baseConversionAmount
            string[] baseConvAmount = baseConversionAmount.Split(' ');
            
            Console.WriteLine(baseConvAmount[3]);
          
            //Convert strings to long
            double baseConvAmountParsed = double.Parse(baseConvAmount[3].Trim(),CultureInfo.InvariantCulture);
            double amountParsed = double.Parse(amount,CultureInfo.InvariantCulture);
            
            //do conversion math
            var expectedResult = baseConvAmountParsed * double.Parse(conversionAmount,CultureInfo.InvariantCulture);
            expectedResult = expectedResult * 1000;
            expectedResult = Math.Ceiling(expectedResult)/1000;
                
            //Assertions
            //compare base amount with displayed amount
            Assert.AreEqual(amountParsed,expectedResult,"Conversion amount does not match base currency value");
            //Compare conversion type to displayed type
            Assert.AreEqual(currencyType,currencyTo,"Currency type does not match chosen conversion currency");
        }
        
        
        public char RandomLetter()
        {
            string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
            Random rand = new Random();
            int num = rand.Next(0, chars.Length -1);
            return chars[num];
        }


        #endregion

    }
}