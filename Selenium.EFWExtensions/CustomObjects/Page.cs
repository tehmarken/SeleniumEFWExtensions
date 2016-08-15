using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace Selenium.EFWExtensions.CustomObjects
{
    public class Page {
        protected IWebDriver _Driver;
        protected WebDriverWait _Wait;
        //constructor for instantiating Page Object models with driver
        public Page(IWebDriver driver) {
            _Driver = driver;
            _Wait = new WebDriverWait(_Driver,TimeSpan.FromSeconds(10));
            PageFactory.InitElements(_Driver, this);
        }
        
        public string PageName { get; set; }
        public string PageUrl { get ; set ; }
        public string PageTitle { get; set; }
        public string MetaDescription { get; set; }
        public string ParentPage { get; set; }

        
    }
}
