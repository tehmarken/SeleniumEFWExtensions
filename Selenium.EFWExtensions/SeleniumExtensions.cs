using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace Selenium.EFWExtensions
{
    public static class SeleniumExtensions
    {
        public static void ScrollToElement(this IWebDriver driver, IWebElement element) {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript(string.Format("window.scrollTo(0,{0});", (element.Location.Y - driver.Manage().Window.Size.Height/2 + element.Size.Height/2)));
        }

        public static int DistanceFromTopOfViewport(this IWebDriver driver, IWebElement element) {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            var dist = (Int64)js.ExecuteScript("return arguments[0] - $(window)['scrollTop']()", element.Location.Y);
            return Convert.ToInt32(dist);
        }
        
        public static void WaitForAJAX(this IWebDriver driver, int sec = 15)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(sec)).Until(d => d.ExecuteJavaScript<bool>("return jQuery.active == 0"));
        }

        public static void WaitForPageload(this IWebDriver driver, int sec = 15)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(sec)).Until(d => d.ExecuteJavaScript<string>("return document.readyState").Equals("complete"));
        }

        public static void WaitASec(this IWebDriver driver, int sec = 1)
        {
            Thread.Sleep(sec*1000);
        }

        public static void WaitForTextInElement(this IWebDriver driver, IWebElement element, string text, int sec = 15)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(sec)).Until(d => element.Text.Contains(text));
        }

        public static bool ElementExists(this IWebDriver driver, By byLocator) {
            var count = driver.FindElements(byLocator).Count;
            return count == 1;
        }

        public static void SelectBySubText(this SelectElement selectElement, string subText)
        {
            foreach (var option in selectElement.Options.Where(option => option.Text.Contains(subText)))
            {
                option.Click();
                return;
            }
            selectElement.SelectByText(subText);
        }

        public static IWebElement FindElementByText(this IWebDriver driver, string text) {
            var elem = driver.FindElement(By.XPath(string.Format("//*[text()='{0}']", text)));
            return elem;
        }

        public static IWebElement FindElementByText(this IWebElement webElement, string text) {
            var elem = webElement.FindElement(By.XPath(string.Format("//*[text()='{0}']", text)));
            return elem;
        }

        public static void SelectOptionByStartsWithText(this IWebElement selectWebElement, string startsWithText) {
            selectWebElement.FindElement(By.XPath(string.Format("./option[starts-with(text(), '{0}')]", startsWithText))).Click();

        }
    }
}
