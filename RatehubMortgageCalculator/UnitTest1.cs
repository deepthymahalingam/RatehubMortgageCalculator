using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RatehubMortgageCalculatorTests
{
    public class Tests
    {
        IWebDriver driver;
        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }
        public double DownPaymentCalculator(int askingPrice)
        {
            if (askingPrice <= 500000)
            {

                return (0.05 * askingPrice);
            }
            if (askingPrice > 500000 && askingPrice < 1000000)
            {
                double downPayment = 25000;
                return downPayment + (0.1 * (askingPrice - 500000));
            }
            else
            {
                return 0.2 * askingPrice;
            }
        }
        public int ExtractDollarValue(string value)
        {
            string dollar = string.Empty;

            for (int i = 0; i < value.Length; i++)
            {
                if (Char.IsDigit(value[i]))
                    dollar += value[i];
            }

            if (dollar.Length > 0)
                return int.Parse(dollar);
            else return 0;
        }

        [Test]
        public void VerifyTitle()
        {
            driver.Url = "https://www.ratehub.ca/mortgage-payment-calculator";
            String Title = driver.Title;
            Console.WriteLine(Title);
            Assert.That(Title, Is.EqualTo("Mortgage Payment Calculator Canada | Ratehub.ca Mortgage Calculator Canada | Calculate Mortgage Payment"));
        }
        [Test]
        public void VerifyDownPaymentValueResets()
        {
            driver.Url = "https://www.ratehub.ca/mortgage-payment-calculator";
            IWebElement askingPrice = driver.FindElement(By.Id("askingPrice"));
            //Ensure allowed minimum for asking price $600,00 is 5.9%
            askingPrice.SendKeys("600000");
            IWebElement scenario0 = driver.FindElement(By.Id("scenarios[0].downPaymentPercent"));
            String scenario0Text = scenario0.GetAttribute("value");
            Assert.That(scenario0Text, Is.EqualTo("5.9%"));
            //Enter a value less than allowed minimum downpayment and ensure the value gets reset to allowed minimum
            scenario0.SendKeys("5.0");
            driver.FindElement(By.Id("scenarios[1].downPaymentPercent")).Click();
            scenario0Text = scenario0.GetAttribute("value");
            Assert.That(scenario0Text, Is.EqualTo("5.9%"));
        }
        [Test]
        public void VerifyDownPaymentCalculation()
        {
            driver.Url = "https://www.ratehub.ca/mortgage-payment-calculator";
            IWebElement askingPrice = driver.FindElement(By.Id("askingPrice"));
            askingPrice.SendKeys("600000");
            IWebElement scenario0Dollars = driver.FindElement(By.Id("scenarios[0].downPaymentDollars"));
            String actualDownPayment = scenario0Dollars.GetAttribute("value");
            Assert.That(Convert.ToDouble(ExtractDollarValue(actualDownPayment)), Is.EqualTo(DownPaymentCalculator(600000)));
        }
        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }
    }
}