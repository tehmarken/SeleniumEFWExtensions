using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;

namespace Selenium.EFWExtensions
{
    public class BaseSeleniumTest {
        
        protected IWebDriver WebDriver;
        public EventFiringWebDriver Driver;
        public WebDriverWait Wait;
        public IList<string> ExceptionScreenshotIgnoreList; 
        public string ExceptionScreenshotDirectory = @"C:\SeleniumLogs\ExceptionScreenshots";

        public enum Browser {
            Chrome,
            Firefox,
            IE
        }
        
        public void EFWDriverSetUp(Browser browser) {
            WebDriver = WebDriverFactory(browser);
            Driver = new EventFiringWebDriver(WebDriver);
            
            //set up a standard Wait with a 10 seconds time-out for conditional waits on webpages
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            ExceptionScreenshotIgnoreList = new List<string>();

            //configure the web driver
            Driver.Manage().Window.Maximize(); //maximize window for optimal view of page, avoid flakiness from smaller view.
            Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(20));//set universal time-out of 20 seconds on page load
            Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));//gives 5 second grace period for element to show up before FindElement throws
            Driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(5));//gives executeAsyncScript 5 seconds grace period to finish executing the javascript before throwing 

            //set up event handling
            Driver.ExceptionThrown += TakeScreenshotOnException;
            Driver.Navigating += LogNavigation;
            Driver.ElementClicking += ScrollToElement;
        }

        public IWebDriver WebDriverFactory(Browser browser) {
            IWebDriver driver = null;
            
            switch (browser) {
                case Browser.Chrome:
                    driver = new ChromeDriver();
                    break;

                case Browser.Firefox:
                    driver = new FirefoxDriver();
                    break;

                case Browser.IE:
                    driver = new InternetExplorerDriver();
                    break;
            }

            return driver;
        }

        [TearDown]
        public void TeardownTest()
        {
            //Dispose driver to make sure it doesn't lock up connection ports
            Driver.Dispose();
            Driver.Quit();
        }

        //special events for use with the EventFiringWebDriver; logging, catching exceptions & taking screnshots, waits, etc.
        
        //if any exception happens in the driver, take a screenshot and save it
        public void TakeScreenshotOnException(object sender, WebDriverExceptionEventArgs e)
        {
            
                if (ExceptionScreenshotIgnoreList.Any(e.ThrownException.Message.Contains)) {
                    //if the exception message contains text that was added to the exception ignore list, screenshot is not taken
                }
                else {
                    Console.WriteLine("{0} | Exception encountered, taking screenshot. Exception: {1}", DateTime.Now.ToString("G"), e.ThrownException);
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_hh-mm-ss");
                    string screenshotFilename = ExceptionScreenshotDirectory + @"\" + timestamp + ".png";
                    
                    //saves the entire screen. since we maximize the browser window during BaseDriverSetUp, this will show the full browser window including url and any other windows that might be displayed
                    Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);//create bitmap image the size of the screen
                    Graphics graphics = Graphics.FromImage(bitmap);//create graphics object from the bitmap that can be drawn to
                    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);//copies every pixel from the current screen to the bitmap via the graphics object
                    graphics.Dispose();//release the bitmap to make sure there's no problem when we try to save it to file

                    if (!Directory.Exists(ExceptionScreenshotDirectory)) {
                        Directory.CreateDirectory(ExceptionScreenshotDirectory);
                    }

                    bitmap.Save(screenshotFilename, ImageFormat.Png);//save the bitmap of the screenshot to file
                    Console.WriteLine("Screenshot saved: {0}", screenshotFilename);
            }
        }

        public virtual void LogNavigation(object sender, WebDriverNavigationEventArgs e)
        {
            Console.WriteLine("Navigating to: {0}", e.Url);
        }

        public void ScrollToElement(object sender, WebElementEventArgs e) {
            //don't scroll if the element is already visible on the page
            if (e.Element.Location.Y < Driver.Manage().Window.Size.Height * .75) return;
            Driver.ScrollToElement(e.Element);
        }
        
    }
}
