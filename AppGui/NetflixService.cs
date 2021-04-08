using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppGui
{
    class NetflixService
    {
        private String url = "https://www.netflix.com/browse";
        private IWebDriver driver;
        private String username, password; // Netflix Credentials

        public NetflixService()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
            login();
        }

        public void login() 
        {
            try {
                driver.FindElement(By.XPath("//input[@id='id_userLoginId']")).SendKeys(username);
                driver.FindElement(By.XPath("//input[@id='id_password']")).SendKeys(password);
                driver.FindElement(By.XPath("//button[@class='btn login-button btn-submit btn-small']")).Click();
                System.Threading.Thread.Sleep(1000);
            }
            catch(InvalidOperationException e)
            {
                return;
            }
            catch(NoSuchElementException e)
            {
                return;
            }
            
        }
        
        public List<String> getAllMoviesSeries()
        {
            List<String> all;
            // driver.FindElement(By.XPath("//*[@id='title-card-2-1']/div[1]/a/div[1]/div/p[@class='fallback-text'] and contains(.,'Mundo Jurássico: Reino Caído')]")).Click();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            //Console.WriteLine(driver.FindElements(By.XPath("//p[@class=\"fallback-text\"]//text()")).Select(iw => iw.Text));

            all = null;
            System.Threading.Thread.Sleep(2000);

            wait.Until(drv => driver.FindElements(By.XPath("//p[@class=\"fallback-text\"]")));

            all = new List<string>(driver.FindElements(By.XPath("//p[@class=\"fallback-text\"]")).Select(iw => iw.Text));
            Console.WriteLine("TOTALLLLL:"+all.Count());
            return all;
        }

        public void choosePerfil(String perfilName)
        {
            try 
            {
                driver.FindElement(By.XPath("//span[contains(., '"+perfilName+"')]")).Click();
                System.Threading.Thread.Sleep(1000);
            }
            catch(InvalidOperationException e) 
            {
                return;
            }
            catch (NoSuchElementException e)
            {
                return;
            }
            
        }

        public void chooseNavBar(String choice) 
        {
            try
            {
                driver.FindElement(By.XPath("//ul[@class='tabbed-primary-navigation']//li[@class='navigation-tab']/a[contains(., '" + choice +"')]")).Click();
                System.Threading.Thread.Sleep(1000);
            }
            catch (InvalidOperationException e)
            {
                return;
            }
            catch (NoSuchElementException e)
            {
                return;
            }
        }

        public void playerControls(String control) 
        {
            Console.WriteLine(control);
            try
            {
                driver.FindElement(By.XPath("//button[@aria-label='"+control+"']")).Click();
                
                
                System.Threading.Thread.Sleep(1000);
            }
            catch (InvalidOperationException e)
            {
                return;
            }
            catch (NoSuchElementException e)
            {
            }
        }

        public void watch(String name)
        {
            Console.WriteLine("AQUIIIIIIIIIIIIIIIIIIII");
            Console.WriteLine(name);
            
            try
            {
                driver.FindElement(By.XPath("//a[contains(., '" + name + "')]")).Click();
                System.Threading.Thread.Sleep(2000);
                driver.FindElement(By.XPath("//button[contains(., 'Ver')]")).Click();
            }
            catch (InvalidOperationException e)
            {
                return;
            }
            catch (NoSuchElementException e)
            {
                return;
            }
        }
    }
}
