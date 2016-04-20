using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaivisVendingMachine;
using System;

namespace Machine.Tests
{
    [TestClass]
    public class MachineTest
    {

        VendingMachine machine;

        [TestInitialize]
        public void TestSetUp()
        {
            //By default, machine with one product
            machine = new VendingMachine();

            machine.AddNewProduct(2, "Fanta", 1, 9);
        }

        //Test method when user first time insert money
        [TestMethod]
        public void InsertOneEuro()
        {
            //Arrange
            //VendingMachine machine = new VendingMachine();

            //Money what will be inserted - act
            Money m1 = new Money();
            m1.Euros = 1;
            m1.Cents = 0;

            Money vendingMoney = machine.InsertCoin(m1);
            //Assert
            Assert.AreEqual(1, vendingMoney.Euros);
            Assert.AreEqual(0, vendingMoney.Cents);
        }

        //User inserts no valid coin
        [TestMethod]
        public void InsertNotValidCoin()
        {
            //Invalid cent
            Money m1 = new Money();
            m1.Cents = 12;
            machine.InsertCoin(m1);
            //Machine doesnt accept - so, no increase should be in Machine money amount
            Assert.AreEqual(0, machine.Amount.Cents);
            
            //Invalid euro
            Money m2 = new Money();
            m2.Euros = 3;
            machine.InsertCoin(m2);

            Assert.AreEqual(0, machine.Amount.Euros);
        }

        //User inserts not valid coin after inserting valid coin
        [TestMethod]
        public void InsertNotValidAfterValidCoin()
        {
            //Invalid cent
            Money m1 = new Money();
            m1.Cents = 10;
            machine.InsertCoin(m1);

            //Invalid euro
            Money m2 = new Money();
            m2.Euros = 3;
            machine.InsertCoin(m2);
            //Check machine amount after one valid and one not valid coin
            Assert.AreEqual(10, machine.Amount.Cents);
            Assert.AreEqual(0, machine.Amount.Euros);
        }

        [TestMethod]
        public void InsertTwoCoins_OneEuroAndTenCents()
        {
            //Money what will be inserted - act
            Money m1 = new Money();
            m1.Euros = 1;
            m1.Cents = 0;

            Money m2 = new Money();
            m2.Euros = 0;
            m2.Cents = 10;

            machine.InsertCoin(m1);
            Money vendingMoney = machine.InsertCoin(m2);
            //Assert
            Assert.AreEqual(1, vendingMoney.Euros);
            Assert.AreEqual(10, vendingMoney.Cents);
        }

        [TestMethod]
        public void ReturnInserted_OneEuroTenCents()
        {
            //Money what will be inserted - act
            Money m1 = new Money();
            m1.Euros = 1;
            m1.Cents = 10;

            machine.InsertCoin(m1);

            Money returnMoneyToUser = machine.ReturnMoney();
            Assert.AreEqual(1, returnMoneyToUser.Euros);
            Assert.AreEqual(10, returnMoneyToUser.Cents);
        }

        [TestMethod]
        public void AddNewProduct_Fanta()
        {
            machine.AddNewProduct(1, "Snicker", 0, 67);

            //Check last product in machine product list
            Assert.AreEqual(1, machine.Products[machine.Products.Length - 1].Available);
            Assert.AreEqual("Snicker", machine.Products[machine.Products.Length - 1].Name);
            Assert.AreEqual(0, machine.Products[machine.Products.Length - 1].Price.Euros);
            Assert.AreEqual(67, machine.Products[machine.Products.Length - 1].Price.Cents);
            //Check if first product is stil in list
            Assert.AreEqual("Fanta", machine.Products[0].Name);
        }

        [TestMethod]
        public void AddNewProductBeforeAndAfterReturnMoney()
        {
            machine.AddNewProduct(1, "Snicker", 0, 67);

            //Check last product in machine product list
            Assert.AreEqual(2, machine.Products.Length);

            machine.ReturnMoney();

            machine.AddNewProduct(2, "Orbit", 0, 23);
            Assert.AreEqual(3, machine.Products.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException),
        "Currency or avaibility cant be negative")]
        public void TryToAddNotValidNewProduct()
        {
            //available amount for new product is 0
            machine.AddNewProduct(0, "Snicker", 0, 67);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException),
        "Currency or avaibility cant be negative")]
        public void TryToAddNotValidNewProduct2()
        {
            //negative price
            machine.AddNewProduct(2, "Snicker", 0, -67);
        }

        [TestMethod]
        public void DeleteProductFromProductList()
        {
            //Add one product
            machine.AddNewProduct(1, "Snicker", 0, 67);
            //Try to delete first product from list
            machine.DeleteProductFromVendingMachine(1);
            //Check if one product left and if its new product.
            Assert.AreEqual(1, machine.Products.Length);
            Assert.AreEqual("Snicker", machine.Products[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException),
        "Product is not find")]
        public void TryToDeleteProductWhatNoExist()
        {
            //By default one product is avaible, try to delete second
            //what not exist
            machine.DeleteProductFromVendingMachine(2);
            Assert.AreEqual(1, machine.Products.Length);
        }

        [TestMethod]
        public void TryToUpdateFirstProduct()
        {
            //Update first - default product availability
            machine.UpdateProductInProductList(1, 10, "Fanta", 1, 9);

            Assert.AreEqual(10, machine.Products[0].Available);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException),
        "Product is not find")]
        public void TryToUpdateProductWhatNotExist()
        {
            //Product 99 not exist, try to update it
            machine.UpdateProductInProductList(99, 5, "SS", 10, 90);

            Assert.AreEqual(5, machine.Products[98].Available);
        }

        [TestMethod]
        public void UserWantsToBuyAFanta()
        {
            //add Fanta to Product list
            machine.AddNewProduct(2, "Snicker", 0, 63);

            //user adds money
            Money m1 = new Money();
            m1.Euros = 1;
            m1.Cents = 10;

            machine.InsertCoin(m1);
            Product machineGive = machine.Buy(2);//Want to buy 
            Assert.AreEqual(2, machine.Products.Length);
            Assert.AreEqual("Snicker", machineGive.Name);
            Assert.AreEqual(1, machineGive.Available);//check if availability is less for one unit

            //Give remainder back 
            Money remainderAfterBuy = machine.lastRemainder;
            Assert.AreEqual(0, remainderAfterBuy.Euros);
            Assert.AreEqual(47, remainderAfterBuy.Cents);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException),
        "Product is not find")]
        public void UserWantsToBuyProductWhatDoNoExist()
        {
            //Add money
            Money m1 = new Money();
            m1.Cents = 20;

            machine.InsertCoin(m1);
            //Try to buy product what no exist, for example, with number 55
            Product machineGive = machine.Buy(55);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException),
        "Product you try to buy is not avaible")]
        public void UserWantsToBuyProductWhatIsNotAvaible()
        {
            machine.AddNewProduct(1, "Mentos", 0, 32);
            //With money
            Money m1 = new Money();
            m1.Cents = 50;
            machine.InsertCoin(m1);
            machine.Buy(2);
            //Buy product what is not avaible
            machine.InsertCoin(m1);
            machine.Buy(2);
        }

        [TestMethod]
        public void FewUsersBuyDifferentProducts()
        {
            //Add two more products to machine
            machine.AddNewProduct(2, "Orbit", 0, 39);
            machine.AddNewProduct(5, "Twix", 0, 73);

            //first user adds money
            Money m1 = new Money();
            m1.Cents = 50;

            machine.InsertCoin(m1);
            Product machineGive = machine.Buy(2);//Want to buy second product from 3
            Assert.AreEqual(3, machine.Products.Length);
            Assert.AreEqual("Orbit", machineGive.Name);
            Assert.AreEqual(1, machineGive.Available);//check if availability is less for one unit

            //Give remainder back 
            Money remainderAfterBuy = machine.lastRemainder;
            Assert.AreEqual(0, remainderAfterBuy.Euros);
            Assert.AreEqual(11, remainderAfterBuy.Cents);

            //Next user buys last product, check, how dat goes
            Money m2 = new Money();
            m2.Euros = 1;

            machine.InsertCoin(m2);
            Product machineGiveSecond = machine.Buy(3);//Wants to buy last product
            Assert.AreEqual(3, machine.Products.Length);
            Assert.AreEqual("Twix", machineGiveSecond.Name);
            Assert.AreEqual(4, machineGiveSecond.Available);//check if availability is less for one unit

            //Give remainder back 
            remainderAfterBuy = machine.lastRemainder;
            Assert.AreEqual(0, remainderAfterBuy.Euros);
            Assert.AreEqual(27, remainderAfterBuy.Cents);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException),
       "Not enough money for buying product")]
        public void UserWantsToBuyProductWithoutEnoughMoney()
        {
            //No money inserted - just buy
            machine.Buy(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException),
       "Not enough money for buying product")]
        public void UserWantsToBuyProductWithoutEnoughMoney2()
        {
            //Not enough money inserted
            Money m1 = new Money();
            m1.Cents = 10;
            machine.InsertCoin(m1);

            machine.Buy(1);
        }

        [TestCleanup]
        public void TestCleanup()//Little cleanup
        {
            machine = null;
        }

    }
}
