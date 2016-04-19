using System;
using System.Linq;
using System.Text;

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
    class VendingMachine : IVendingMachine
    {
        //Variables
        Product[] products = new Product[] { };
        string manufacturer = "Raivis";
        Money totalBilance = new Money();
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

            try
            {   
                if(realProductNumber >= 0 && realProductNumber < Products.Length)//Check if index is in bounds
                {
                    //Fist check, if this product is avaible
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
                            //Console.WriteLine("Cash out remainder: " + euroLeft + " \u20AC " + centsLeft + " \u00A2");
                            ReturnMoney();

                            //Give product to user
                            Console.WriteLine("Product " + "\'" + Products[realProductNumber].Name + "\'" + " drops out" + "\nThank you for your purchase!");
                            return Products[realProductNumber];
                        }
                        else
                        {
                            //Info user about not enough coins
                            //ReturnMoney(); - Maybe there is not need to drop out money, if user has not enough money, can buy something else
                            throw new InvalidOperationException("Not enough money for buying product");
                        }
                    }
                    else
                    {
                        //Not enough products
                        ReturnMoney();
                        throw new InvalidOperationException("Product you try to buy is not avaible");
                    }
                }
                else//Product is not found
                {
                    ReturnMoney();
                    throw new IndexOutOfRangeException("Product you try to buy is not find");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Paying problem: " + e.Message);
                return new Product();
            }
        }

        public Money InsertCoin(Money amount)
        {
            //Increase user total bilance
            totalBilance.Euros += amount.Euros;
            totalBilance.Cents += amount.Cents;
            return totalBilance;
        }

        public Money ReturnMoney()
        {
            //Print how big sum gives back and zero down.
            if(totalBilance.Cents> 0 || totalBilance.Euros > 0)
            {
                Console.WriteLine("Coins back: " + totalBilance.Euros + " \u20AC " + totalBilance.Cents + " \u00A2");
            }

            totalBilance.Euros = 0;
            totalBilance.Cents = 0;
            //Return new bilance
            return totalBilance;
        }
        //Function used to add products to list
        public void AddNewProduct()
        {
            try
            {
                Console.WriteLine("New product input!\nName Availity Euro Cents");
                string newProductDescription = Console.ReadLine();
                //Trim spaces and split user input to get information about new product                                  
                string[] newProductInfo = newProductDescription.Trim().Split(' ');

                //Little test, if user input new product in correct format
                if(newProductInfo.Length != 4)
                {
                    throw new FormatException("One of new product input fields is not correct completed");
                }

                //Create new product object and store all necesarry information about product from user input
                Product newProduct = new Product();
                newProduct.Available = int.Parse(newProductInfo[1]);
                newProduct.Name = newProductInfo[0];
                //Price
                Money newProductPrice = new Money();
                
                if (int.Parse(newProductInfo[2]) >= 0 && int.Parse(newProductInfo[3]) >= 0 && int.Parse(newProductInfo[1]) > 0)
                {
                    newProductPrice.Euros = int.Parse(newProductInfo[2]);
                    newProductPrice.Cents = int.Parse(newProductInfo[3]);
                }
                else
                {
                    throw new ArithmeticException("Currency or Avaibility is not correctly filled.");
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
            catch (Exception e)//We dont know what can gone wrong - so multi Exception
            {
                Console.WriteLine("Problem adding new product to list: " + e.Message);
            }
        }
        //Overload AddNewProduct - this is for default products
        public void AddNewProduct(int avaibility, string productName, int euro, int cents)
        {
            try
            {
                //New product
                Product newProduct = new Product();
                newProduct.Available = avaibility;
                newProduct.Name = productName;
                //Price for product
                Money newProductPrice = new Money();

                if(euro >= 0 && cents >= 0 && avaibility > 0)
                {
                    newProductPrice.Euros = euro;
                    newProductPrice.Cents = cents;
                }
                else
                {
                    throw new ArithmeticException("Currency cant be negative");
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
            catch (Exception e)//We dont know what can gone wrong - so multi Exception
            {
                Console.WriteLine("Problem adding new product to list: " + e.Message);
            }
        }
    }
    //Main app class
    class MainClass
    {
        static void Main(string[] args)
        {
            //Create instance to Vending machine class
            VendingMachine machine = new VendingMachine();
            //Add default product
            machine.AddNewProduct(3, "Snicker", 0, 62);

            //bool what tells if machine is on or off
            bool isMachineOn = true;
            //Store total bilance - to display on screen
            Money userBilance = machine.Amount;

            //--- Console outputs with working info - machine is turning on!
            Console.OutputEncoding = Encoding.UTF8;//Change console encoding to display euro sign!
            Console.WriteLine("Manufacturer: " + machine.Manufacturer);
            Console.WriteLine("Welcome to Vending Machine!" + "\n\nWhat you want to do?" + " Choose operation - write activity number");
            Console.WriteLine("\n1.Insert money" + "\n2.Choose product" + "\n3.Get coins back" + "\n4.Quit application"
                + "\n100N.Add new product");//Little bit different number for adding new product

            //While machine status is 'true' - machine is working
            while (isMachineOn)
            {
                //Before every user activity - show bilance
                Console.WriteLine("Total bilance is: " + userBilance.Euros + " \u20AC " + userBilance.Cents + " \u00A2");
                //According to user input - do specific activity
                string userInput = Console.ReadLine();
                switch (userInput)//According to user input - do certain activity
                {
                    case "1"://Insert coin
                        Console.WriteLine("Insert coin!");
                        //To store our user coin
                        Money insertCoin = new Money();
                        string userCoinInput = Console.ReadLine();
                        //Little test - if user insert cent, euro or not validate coin
                        if (userCoinInput == "5" || userCoinInput == "10" || userCoinInput == "20" || userCoinInput == "50")
                        {
                            insertCoin.Cents = int.Parse(userCoinInput);
                        }
                        else if (userCoinInput == "1" || userCoinInput == "2")
                        {
                            insertCoin.Euros = int.Parse(userCoinInput);
                        }
                        else if (userCoinInput == "100N")
                        {
                            try
                            {
                                machine.AddNewProduct();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Problem adding new product: " + e.Message);
                            }
                        }
                        else
                        {
                            //invalid currency - throw it back
                            Console.WriteLine("Invalid currency! - coin back!");
                            break;
                        }
                        //Insert coin to machine and update MainClass bilance variable
                        userBilance = machine.InsertCoin(insertCoin);
                        break;
                    case "2"://Buy a product
                        int activeProductIndex = 0;//To display number for product
                        bool haveProductToBuy = false;//To make sure, if we have any product to buy
                        //First display products
                        foreach (var product in machine.Products)
                        {
                            ++activeProductIndex;//Increase index
                            //Make sure if this product is even avaible
                            if (product.Available <= 0)
                            {
                                //Move to next product...
                                continue;
                            }
                            //We can show this product as avaible product
                            haveProductToBuy = true;//We have product to buy
                            Console.WriteLine(activeProductIndex + " " + product.Name + " Price: " + product.Price.Euros + "." + product.Price.Cents + "\u20AC" + " Avaible: " + product.Available);
                        }
                        //According to if machine have products to sell - do next activities
                        if (haveProductToBuy)
                        {
                            //If we have - ask user to one product
                            Console.WriteLine("\nChoose what you want to buy! - Enter number");
                            string userWants = Console.ReadLine();
                            //Little check, if user even inputs number! And if input is not empty
                            if (userWants.All(char.IsDigit) && !String.IsNullOrEmpty(userWants))
                            {
                                //user is trying to buy this product
                                machine.Buy(int.Parse(userWants));
                                //Update bilance
                                userBilance = machine.Amount;

                                //Check is some product is out of avaibility
                                machine.Products = machine.Products.Where(val => val.Available > 0).ToArray();
                            }
                            else//User choose number what is not a digit
                            {
                                Console.WriteLine("You need to choose digit - product number!");
                            }
                        }
                        else//User have not any product to buy!
                        {
                            Console.WriteLine("No products avaible to buy. Please visit later!");
                        }
                        break;
                    case "3"://Return money
                        userBilance = machine.ReturnMoney();//Call machine function what returns money
                        break;
                    case "4"://Case if want to turn machine off
                        isMachineOn = false;
                        break;
                    case "100N"://Case for adding new product to list
                        try
                        {
                            machine.AddNewProduct();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Problem adding new product: " + e.Message);
                        }
                        break;
                    default://If user inputs something else
                        Console.WriteLine("Unknow operation");
                        break;
                }
                //Remainder to choose operation
                Console.WriteLine("\nChoose operation - write activity number");
            }
        }
    }
}
