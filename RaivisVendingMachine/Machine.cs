using System;
using System.Linq;

namespace RaivisVendingMachine
{
    public interface IVendingMachine
    {
        /// <summary>Vending machine manufacturer.</summary>
        string Manufacturer { get; }
        /// <summary>Amount of money inserted into vending machine. </summary>
        Money Amount { get; }
        /// <summary>Products that are sold.</summary>
        Product[] Products { get; set; }
        /// <summary>Inserts the coin into vending machine.</summary>
        /// <param name="amount">Coin amount.</param>
        Money InsertCoin(Money amount);
        /// <summary>Returns all inserted coins back to user.</summary>
        Money ReturnMoney();
        /// <summary>Buys product from list of product.</summary>
        /// <param name="productNumber">Product number in vending machine productlist.</param>
        Product Buy(int productNumber);
    }
    /// <summary> Money describes Euros and cents </summary>
    public struct Money
    {
        public int Euros { get; set; }
        public int Cents { get; set; }
    }

    public struct Product
    {
        /// <summary>Gets or sets the available amount of product.</summary>
        public int Available { get; set; }
        /// <summary>Gets or sets the product price.</summary>
        public Money Price { get; set; }
        /// <summary>Gets or sets the product name.</summary>
        public string Name { get; set; }
    }
    //Vending  machine class to store and add logic to IVendingMachine
    public class VendingMachine : IVendingMachine
    {
        //Variables
        Product[] products = new Product[] { };//Store products for specific VendingMachine object
        string manufacturer = "Raivis";
        Money totalBilance = new Money();
        //Variable what stores last remainder - to check, if is valide
        public Money lastRemainder = new Money();
        //-----
        public Money Amount
        {
            get { return totalBilance; }
        }

        public string Manufacturer
        {
            get
            {
                return manufacturer;
            }
        }

        public Product[] Products
        {
            get
            {
                return products;
            }

            set
            {
                products = value;
            }
        }

        public Product Buy(int productNumber)
        {
            //Working with array - product number will be -1
            int realProductNumber = productNumber - 1;

            IsProductAvaible(realProductNumber);

            //First check, if this product is avaible
            if (Products[realProductNumber].Available > 0)
            {
                //Now check, if user have enough money to buy this product
                if (((Amount.Euros * 100) + Amount.Cents) >= ((Products[realProductNumber].Price.Euros * 100) + Products[realProductNumber].Price.Cents))
                {
                    //--Give product and cash out coin remainder
                    //One product less 
                    Products[realProductNumber].Available -= 1;
                    //Remainder after buy
                    decimal totalCentsLeft = ((Amount.Euros * 100) + Amount.Cents) - ((Products[realProductNumber].Price.Euros * 100) + Products[realProductNumber].Price.Cents);
                    int euroLeft = (int)Math.Floor(totalCentsLeft / 100);
                    int centsLeft = (int)totalCentsLeft % 100;
                    //Set new bilance
                    totalBilance.Euros = euroLeft;
                    totalBilance.Cents = centsLeft;
                    //Return money
                    ReturnMoney();

                    //Give product to user
                    return Products[realProductNumber];
                }
                else
                {
                    //Info user about not enough coins
                    ReturnMoney(); //Maybe there is not need to drop out money, if user has not enough money, can buy something else
                    throw NewInvalidOperationException("Not enough money for buying product");
                }
            }
            else
            {
                //Not enough products
                ReturnMoney();
                throw NewIndexOutOfRangeException("Product you try to buy is not avaible");
            }
        }

        public Money InsertCoin(Money amount)
        {
            //Increase user total bilance
            //Machine accepts only specific coins
            if (amount.Cents == 5 || amount.Cents == 10 || amount.Cents == 20 || amount.Cents == 50)
            {
                totalBilance.Cents += amount.Cents;
            }

            if (amount.Euros == 1 || amount.Euros == 2)
            {
                totalBilance.Euros += amount.Euros;
            }

            return totalBilance;
        }

        public Money ReturnMoney()
        {
            //Return money and set remainder
            lastRemainder = totalBilance; 

            totalBilance.Euros = 0;
            totalBilance.Cents = 0;

            return lastRemainder;
        }
        //--Updating Product list
        //Function to add new product to product list
        public void AddNewProduct(int avaibility, string productName, int euro, int cents)
        {
            //New product
            Product newProduct = new Product();
            newProduct.Available = avaibility;
            newProduct.Name = productName;
            //Price for product
            Money newProductPrice = new Money();

            if (euro >= 0 && cents >= 0 && avaibility > 0)
            {
                newProductPrice.Euros = euro;
                newProductPrice.Cents = cents;
            }
            else
            {
                throw NewArgumentOutOfRangeException("Currency or avaibility cant be negative");
            }

            newProduct.Price = newProductPrice;
            //Define array - what will store all products what is now store and new product
            Product[] updateArray = new Product[Products.Length + 1];
            //Copy old product
            for (int i = 0; i < Products.Length; i++)
            {
                updateArray[i] = Products[i];
            }
            //Add new product to this array
            updateArray[Products.Length] = newProduct;
            //Change vending machine product array to new one
            Products = updateArray;
        }

        //Function to update product in product list
        public void UpdateProductInProductList(int productNumber, int avaibility, string productName, int euro, int cents)
        {
            int productNumberInArray = productNumber - 1;

            IsProductAvaible(productNumberInArray);

            Product[] updateArray = new Product[Products.Length];
            //Copy old product - if find Update product, change it
            for (int i = 0; i < Products.Length; i++)
            {
                if(i == productNumberInArray)
                {
                    Products[i].Available = avaibility;
                    Products[i].Name = productName;

                    Money newPrice = new Money();
                    newPrice.Euros = euro;
                    newPrice.Cents = cents;

                    Products[i].Price = newPrice;

                    updateArray[i] = Products[i];
                }
                else
                {
                    updateArray[i] = Products[i];
                }
            }
            //Change vending machine product array to new one
            Products = updateArray;
            
        }
        //To delete Product from list
        public void DeleteProductFromVendingMachine(int productNumber)
        {
            //Check if avaible
            IsProductAvaible(productNumber - 1);
            //Convert array to list
            var numbersList = Products.ToList();
            //Remove element with specific index
            numbersList.RemoveAt(productNumber - 1);
            //Convert back and assign to Products
            Products = numbersList.ToArray();
        }
        //--end of product list updating functions

        //Little function to check, if product is avaible
        void IsProductAvaible(int productNumber)
        {
            if (productNumber < 0 || productNumber > Products.Length-1)
            {
                throw NewIndexOutOfRangeException("Product is not find");
            }
        }

        //Different type of exceptions -----------
        IndexOutOfRangeException NewIndexOutOfRangeException(string message)
        {
            return new IndexOutOfRangeException(message);
        }

        InvalidOperationException NewInvalidOperationException(string message)
        {
            return new InvalidOperationException(message);
        }

        ArgumentOutOfRangeException NewArgumentOutOfRangeException(string message)
        {
            return new ArgumentOutOfRangeException(message);
        }
        //--Excepion end 
    }
}
