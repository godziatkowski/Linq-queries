// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;   
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;

using LINQTest;

// Version Mad01

namespace SampleQueries {
    [Title("101 LINQ Query Samples")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness 
    {        
        private readonly static string dataPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Data\"));
        
        # region Sample Data Collections

        // Sample objects based on Northwnd data, 
        // but not identical to it in all cases.

        public class Customer
        {
            public string CustomerID { get; set; }
            public string CompanyName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public Order[] Orders { get; set; }
        }

        public class Order
        {
            public int OrderID { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
        }

        public class Supplier
        {
            public string SupplierName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
        }
        
        private List<Product> productList;
        private List<Customer> customerList;
        private List<Supplier> supplierList;

        #endregion

        [Category("Restriction Operators")]
        [Title("Where - Simple 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;
        
            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums) {
                Console.WriteLine(x);
            }
        }
        
        [Category("Restriction Operators")]
        [Title("Where - Simple 2")]
        [Description("This sample uses the where clause to find all products that are out of stock.")]
        public void Linq2() {
            List<Product> products = GetProductList();

            var soldOutProducts =
                from prod in products
                where prod.UnitsInStock == 0
                select prod;
            
            Console.WriteLine("Sold out products:");
            foreach (var product in soldOutProducts) {
                Console.WriteLine("{0} is sold out!", product.ProductName);
            }
        }
        
        [Category("Restriction Operators")]
        [Title("Where - Simple 3")]
        [Description("This sample uses the where clause to find all products that are in stock and " +
                     "cost more than 3.00 per unit.")]
        public void Linq3() {
            List<Product> products = GetProductList();

            var expensiveInStockProducts =
                from prod in products
                where prod.UnitsInStock > 0 && prod.UnitPrice > 3.00
                select prod;
            
            Console.WriteLine("In-stock products that cost more than 3.00:");
            foreach (var product in expensiveInStockProducts) {
                Console.WriteLine("{0} is in stock and costs more than 3.00.", product.ProductName);
            }
        }
        
        [Category("Restriction Operators")]
        [Title("Where - Drilldown")]
        [Description("This sample uses the where clause to find all customers in Washington " +
                     "and then it uses a foreach loop to iterate over the orders collection that belongs to each customer.")]
        public void Linq4() {
            List<Customer> customers = GetCustomerList();

            var waCustomers =
                from cust in customers
                where cust.Region == "WA"
                select cust;

            Console.WriteLine("Customers from Washington and their orders:");
            foreach (var customer in waCustomers)
            {
                Console.WriteLine("Customer {0}: {1}", customer.CustomerID, customer.CompanyName);
                foreach (var order in customer.Orders)
                {
                    Console.WriteLine("  Order {0}: {1}", order.OrderID, order.OrderDate);
                }
            }
        }
        
        [Category("Restriction Operators")]
        [Title("Where - Indexed")]
        [Description("This sample demonstrates an indexed where clause that returns digits whose name is " +
                    "shorter than their value.")]
        public void Linq5() {
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var shortDigits = digits.Where((digit, index) => digit.Length < index);
        
            Console.WriteLine("Short digits:");
            foreach (var d in shortDigits)
            {
                Console.WriteLine("The word {0} is shorter than its value.", d);
            }
        }

        [Category("Projection Operators")]
        [Title("Select - Simple 1")]
        [Description("This sample uses the select clause to produce a sequence of ints one higher than " +
                     "those in an existing array of ints.")]
        public void Linq6() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numsPlusOne =
                from num in numbers
                select num + 1;
            
            Console.WriteLine("Numbers + 1:");
            foreach (var i in numsPlusOne) {
                Console.WriteLine(i);
            }
        } 
        
        [Category("Projection Operators")]
        [Title("Select - Simple 2")]
        [Description("This sample uses the select clause to return a sequence of product names.")]
        public void Linq7() {
            List<Product> products = GetProductList();

            var productNames =
                from prod in products
                select prod.ProductName;
            
            Console.WriteLine("Product Names:");
            foreach (var productName in productNames) {
                Console.WriteLine(productName);
            }
        }

        [Category("Projection Operators")]
        [Title("Select - Transformation")]
        [Description("This sample uses the select clause to produce a sequence of strings representing " +
                     "the text version of a sequence of ints.")]
        public void Linq8() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var textNums = 
                from num in numbers
                select strings[num];
            
            Console.WriteLine("Number strings:");
            foreach (var str in textNums) {
                Console.WriteLine(str);
            }           
        }

        [Category("Projection Operators")]
        [Title("Select - Anonymous Types 1")]
        [Description("This sample uses the select clause to produce a sequence of the uppercase " +
                     "and lowercase versions of each word in the original array.")]
        public void Linq9() {
            string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };

            var upperLowerWords =
                from word in words
                select new {Upper = word.ToUpper(), Lower = word.ToLower()};

            foreach (var wordPair in upperLowerWords) {
                Console.WriteLine("Uppercase: {0}, Lowercase: {1}", wordPair.Upper, wordPair.Lower);
            }
        }

        [Category("Projection Operators")]
        [Title("Select - Anonymous Types 2")]
        [Description("This sample uses the select clause to produce a sequence containing text " +
                     "representations of digits and a Boolean that specifies whether the text length is even or odd.")]
        public void Linq10() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var digitOddEvens =
                from num in numbers
                select new {Digit = strings[num], Even = (num % 2 == 0)};

            foreach (var digit in digitOddEvens) {
                Console.WriteLine("The digit {0} is {1}.", digit.Digit, digit.Even ? "even" : "odd");
            }
        }
        
        [Category("Projection Operators")]
        [Title("Select - Anonymous Types 3")]
        [Description("This sample uses the select clause to produce a sequence containing some properties " +
                     "of Products, including UnitPrice which is renamed to Price " +
                     "in the resulting type.")]
        public void Linq11() {
            List<Product> products = GetProductList();

            var productInfos =
                from prod in products
                select new {prod.ProductName, prod.Category, Price = prod.UnitPrice};
            
            Console.WriteLine("Product Info:");
            foreach (var productInfo in productInfos) {
                Console.WriteLine("{0} is in the Category {1} and costs {2} per unit.", productInfo.ProductName, productInfo.Category, productInfo.Price);
            }
        } 

        [Category("Projection Operators")]
        [Title("Select - Indexed")]
        [Description("This sample uses an indexed Select clause to determine if the value of ints " +
                     "in an array match their position in the array.")]
        public void Linq12() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numsInPlace = numbers.Select((num, index) => new {Num = num, InPlace = (num == index)});
        
            Console.WriteLine("Number: In-place?");
            foreach (var n in numsInPlace) {
                Console.WriteLine("{0}: {1}", n.Num, n.InPlace);
            }
        }

        [Category("Projection Operators")]
        [Title("Select - Filtered")]
        [Description("This sample combines select and where to make a simple query that returns " +
                     "the text form of each digit less than 5.")]
        public void Linq13() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var lowNums =
                from num in numbers
                where num < 5
                select digits[num];
            
            Console.WriteLine("Numbers < 5:");
            foreach (var num in lowNums) {
                Console.WriteLine(num);
            }       
        }
        
        [Category("Projection Operators")]
        [Title("SelectMany - Compound from 1")]
        [Description("This sample uses a compound from clause to make a query that returns all pairs " +
                     "of numbers from both arrays in which the number from numbersA is less than the number " +
                     "from numbersB.")]
        public void Linq14() {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            var pairs =
                from a in numbersA
                from b in numbersB
                where a < b
                select new {a, b};
        
            Console.WriteLine("Pairs where a < b:");
            foreach (var pair in pairs) {
                Console.WriteLine("{0} is less than {1}", pair.a, pair.b);
            }
        }
        
        [Category("Projection Operators")]
        [Title("SelectMany - Compound from 2")]
        [Description("This sample uses a compound from clause to select all orders where the " +
                     "order total is less than 500.00.")]
        public void Linq15() {
            List<Customer> customers = GetCustomerList();

            var orders =
                from cust in customers
                from order in cust.Orders
                    where order.Total < 500.00M
                    select new {cust.CustomerID, order.OrderID, order.Total};
        
            ObjectDumper.Write(orders);
        }

        [Category("Projection Operators")]
        [Title("SelectMany - Compound from 3")]
        [Description("This sample uses a compound from clause to select all orders where the " +
                     "order was made in 1998 or later.")]
        public void Linq16() {
            List<Customer> customers = GetCustomerList();

            var orders =
                from cust in customers
                from order in cust.Orders
                    where order.OrderDate >= new DateTime(1998, 1, 1)
                    select new {cust.CustomerID, order.OrderID, order.OrderDate};
        
            ObjectDumper.Write(orders);
        }

        [Category("Projection Operators")]
        [Title("SelectMany - With let")]
        [Description("This sample uses a compound from clause to select all orders where the " +
                     "order total is greater than 2000.00 and uses a let clause to avoid " +
                     "requesting the total twice.")] 
        public void Linq17() {
            List<Customer> customers = GetCustomerList();

            var orders =
                from cust in customers
                from order in cust.Orders
                let total = order.Total
                where total >= 2000.0M
                select new {cust.CustomerID, order.OrderID, total};

            ObjectDumper.Write(orders);
        }
    
        [Category("Projection Operators")]
        [Title("SelectMany - Compound from")]
        [Description("This sample uses compound from clauses so that filtering on customers can " +
                     "be done before selecting their orders.  This makes the query more efficient by " +
                     "not selecting and then discarding orders for customers outside of Washington.")]
        public void Linq18() {
            List<Customer> customers = GetCustomerList();

            DateTime cutoffDate = new DateTime(1997, 1, 1);

            var orders =
                from cust in customers
                where cust.Region == "WA"
                from order in cust.Orders
                    where order.OrderDate >= cutoffDate
                    select new {cust.CustomerID, order.OrderID};
        
            ObjectDumper.Write(orders);
        }
        
        [Category("Projection Operators")]
        [Title("SelectMany - Indexed")]
        [Description("This sample uses an indexed SelectMany clause to select all orders, " +
                     "while referring to customers by the order in which they are returned " +
                     "from the query.")]
        public void Linq19() {
            List<Customer> customers = GetCustomerList();

            var customerOrders =
                customers.SelectMany(
                    (cust, custIndex) =>
                    cust.Orders.Select(o => "Customer #" + (custIndex + 1) +
                                            " has an order with OrderID " + o.OrderID));

            ObjectDumper.Write(customerOrders);
        }
        
        [Category("Partitioning Operators")]
        [Title("Take - Simple")]
        [Description("This sample uses Take to get only the first 3 elements of " +
                     "the array.")]
        public void Linq20() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            var first3Numbers = numbers.Take(3);
            
            Console.WriteLine("First 3 numbers:");
            foreach (var n in first3Numbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Partitioning Operators")]
        [Title("Take - Nested")]
        [Description("This sample uses Take to get the first 3 orders from customers " +
                     "in Washington.")]
        public void Linq21() {
            List<Customer> customers = GetCustomerList();

            var first3WAOrders = (
                from cust in customers
                from order in cust.Orders
                where cust.Region == "WA"
                select new {cust.CustomerID, order.OrderID, order.OrderDate} )
                .Take(3);
            
            Console.WriteLine("First 3 orders in WA:");
            foreach (var order in first3WAOrders) {
                ObjectDumper.Write(order);
            }
        }

        [Category("Partitioning Operators")]
        [Title("Skip - Simple")]
        [Description("This sample uses Skip to get all but the first four elements of " +
                     "the array.")]
        public void Linq22() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            var allButFirst4Numbers = numbers.Skip(4);
            
            Console.WriteLine("All but first 4 numbers:");
            foreach (var n in allButFirst4Numbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Partitioning Operators")]
        [Title("Skip - Nested")]
        [Description("This sample uses Take to get all but the first 2 orders from customers " +
                     "in Washington.")]
        public void Linq23() {
            List<Customer> customers = GetCustomerList();

            var waOrders =
                from cust in customers
                from order in cust.Orders
                where cust.Region == "WA"
                select new {cust.CustomerID, order.OrderID, order.OrderDate};
        
            var allButFirst2Orders = waOrders.Skip(2);
            
            Console.WriteLine("All but first 2 orders in WA:");
            foreach (var order in allButFirst2Orders) {
                ObjectDumper.Write(order);
            }
        }    
            
        [Category("Partitioning Operators")]
        [Title("TakeWhile - Simple")]
        [Description("This sample uses TakeWhile to return elements starting from the " +
                     "beginning of the array until a number is read whose value is not less than 6.")]
        public void Linq24() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            var firstNumbersLessThan6 = numbers.TakeWhile(n => n < 6);
            
            Console.WriteLine("First numbers less than 6:");
            foreach (var num in firstNumbersLessThan6) {
                Console.WriteLine(num);
            }
        }
        
        [Category("Partitioning Operators")]
        [Title("TakeWhile - Indexed")]
        [Description("This sample uses TakeWhile to return elements starting from the " +
                    "beginning of the array until a number is hit that is less than its position " +
                    "in the array.")]
        public void Linq25() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            var firstSmallNumbers = numbers.TakeWhile((n, index) => n >= index);
            
            Console.WriteLine("First numbers not less than their position:");
            foreach (var n in firstSmallNumbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Partitioning Operators")]
        [Title("SkipWhile - Simple")]
        [Description("This sample uses SkipWhile to get the elements of the array " +
                    "starting from the first element divisible by 3.")]
        public void Linq26() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            // In the lambda expression, 'n' is the input parameter that identifies each
            // element in the collection in succession. It is is inferred to be
            // of type int because numbers is an int array.
            var allButFirst3Numbers = numbers.SkipWhile(n => n % 3 != 0);
            
            Console.WriteLine("All elements starting from first element divisible by 3:");
            foreach (var n in allButFirst3Numbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Partitioning Operators")]
        [Title("SkipWhile - Indexed")]
        [Description("This sample uses SkipWhile to get the elements of the array " +
                    "starting from the first element less than its position.")]
        public void Linq27() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            var laterNumbers = numbers.SkipWhile((n, index) => n >= index);
            
            Console.WriteLine("All elements starting from first element less than its position:");
            foreach (var n in laterNumbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Ordering Operators")]
        [Title("OrderBy - Simple 1")]
        [Description("This sample uses orderby to sort a list of words alphabetically.")]
        public void Linq28() {
            string[] words = { "cherry", "apple", "blueberry" };
            
            var sortedWords =
                from word in words
                orderby word
                select word;
            
            Console.WriteLine("The sorted list of words:");
            foreach (var w in sortedWords) {
                Console.WriteLine(w);
            }
        }

        [Category("Ordering Operators")]
        [Title("OrderBy - Simple 2")]
        [Description("This sample uses orderby to sort a list of words by length.")]
        public void Linq29() {
            string[] words = { "cherry", "apple", "blueberry" };
            
            var sortedWords =
                from word in words
                orderby word.Length
                select word;
            
            Console.WriteLine("The sorted list of words (by length):");
            foreach (var w in sortedWords) {
                Console.WriteLine(w);
            }
        }
        
        [Category("Ordering Operators")]
        [Title("OrderBy - Simple 3")]
        [Description("This sample uses orderby to sort a list of products by name. " +
                    "Use the \"descending\" keyword at the end of the clause to perform a reverse ordering.")]
        public void Linq30() {
            List<Product> products = GetProductList();

            var sortedProducts =
                from prod in products
                orderby prod.ProductName
                select prod;
        
            ObjectDumper.Write(sortedProducts);
        }

        // Custom comparer for use with ordering operators
        public class CaseInsensitiveComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Category("Ordering Operators")]
        [Title("OrderBy - Comparer")]
        [Description("This sample uses an OrderBy clause with a custom comparer to " +
                     "do a case-insensitive sort of the words in an array.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void Linq31() {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"};
            
            var sortedWords = words.OrderBy(a => a, new CaseInsensitiveComparer());
                
            ObjectDumper.Write(sortedWords);
        }
        
        [Category("Ordering Operators")]
        [Title("OrderByDescending - Simple 1")]
        [Description("This sample uses orderby and descending to sort a list of " +
                     "doubles from highest to lowest.")]
        public void Linq32() {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };
            
            var sortedDoubles =
                from d in doubles
                orderby d descending
                select d;
            
            Console.WriteLine("The doubles from highest to lowest:");
            foreach (var d in sortedDoubles) {
                Console.WriteLine(d);
            }
        }
        
        [Category("Ordering Operators")]
        [Title("OrderByDescending - Simple 2")]
        [Description("This sample uses orderby to sort a list of products by units in stock " +
                     "from highest to lowest.")]
        public void Linq33() {
            List<Product> products = GetProductList();

            var sortedProducts =
                from prod in products
                orderby prod.UnitsInStock descending
                select prod;
        
            ObjectDumper.Write(sortedProducts);
        }

        [Category("Ordering Operators")]
        [Title("OrderByDescending - Comparer")]
        [Description("This sample uses method syntax to call OrderByDescending because it " +
                    " enables you to use a custom comparer.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void Linq34() {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"};
            
            var sortedWords = words.OrderByDescending(a => a, new CaseInsensitiveComparer());
                
            ObjectDumper.Write(sortedWords);
        }
        
        [Category("Ordering Operators")]
        [Title("ThenBy - Simple")]
        [Description("This sample uses a compound orderby to sort a list of digits, " +
                     "first by length of their name, and then alphabetically by the name itself.")]
        public void Linq35() {
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var sortedDigits =
                from digit in digits 
                orderby digit.Length, digit
                select digit;
        
            Console.WriteLine("Sorted digits:");
            foreach (var d in sortedDigits) {
                Console.WriteLine(d);
            }
        }

        [Category("Ordering Operators")]
        [Title("ThenBy - Comparer")]
        [Description("The first query in this sample uses method syntax to call OrderBy and ThenBy with a custom comparer to " +
                     "sort first by word length and then by a case-insensitive sort of the words in an array. " +
                     "The second two queries show another way to perform the same task.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void Linq36() {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"};
            
            var sortedWords =
                words.OrderBy(a => a.Length)
                     .ThenBy(a => a, new CaseInsensitiveComparer());

            // Another way. TODO is this use of ThenBy correct? It seems to work on this sample array.
            var sortedWords2 =
                from word in words
                orderby word.Length
                select word;

            var sortedWords3 = sortedWords2.ThenBy(a => a, new CaseInsensitiveComparer());
                
            ObjectDumper.Write(sortedWords);

            ObjectDumper.Write(sortedWords3);
        }
        
        [Category("Ordering Operators")]
        [Title("ThenByDescending - Simple")]
        [Description("This sample uses a compound orderby to sort a list of products, " +
                     "first by Category, and then by unit price, from highest to lowest.")]
        public void Linq37() {
            List<Product> products = GetProductList();

            var sortedProducts =
                from prod in products
                orderby prod.Category, prod.UnitPrice descending
                select prod;
        
            ObjectDumper.Write(sortedProducts);
        }

        [Category("Ordering Operators")]
        [Title("ThenByDescending - Comparer")]
        [Description("This sample uses an OrderBy and a ThenBy clause with a custom comparer to " +
                     "sort first by word length and then by a case-insensitive descending sort " +
                     "of the words in an array.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void Linq38() {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"};
            
            var sortedWords =
                words.OrderBy(a => a.Length)
                     .ThenByDescending(a => a, new CaseInsensitiveComparer());
                
            ObjectDumper.Write(sortedWords);
        }
        
        [Category("Ordering Operators")]
        [Title("Reverse")]
        [Description("This sample uses Reverse to create a list of all digits in the array whose " +
                     "second letter is 'i' that is reversed from the order in the original array.")]
        public void Linq39() {
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            
            var reversedIDigits = (
                from digit in digits
                where digit[1] == 'i'
                select digit)
                .Reverse();
            
            Console.WriteLine("A backwards list of the digits with a second character of 'i':");
            foreach (var d in reversedIDigits) {
                Console.WriteLine(d);
            }             
        }

        [Category("Grouping Operators")]
        [Title("GroupBy - Simple 1")]
        [Description("This sample uses group by to partition a list of numbers by " +
                    "their remainder when divided by 5.")]
        public void Linq40() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            var numberGroups =
                from num in numbers
                group num by num % 5 into numGroup
                select new { Remainder = numGroup.Key, Numbers = numGroup };
            
            foreach (var grp in numberGroups) {
                Console.WriteLine("Numbers with a remainder of {0} when divided by 5:", grp.Remainder);
                foreach (var n in grp.Numbers) {
                    Console.WriteLine(n);
                }
            }
        }
        
        [Category("Grouping Operators")]
        [Title("GroupBy - Simple 2")]
        [Description("This sample uses group by to partition a list of words by " +
                     "their first letter.")]
        public void Linq41() {
            string[] words = { "blueberry", "chimpanzee", "abacus", "banana", "apple", "cheese" };
            
            var wordGroups =
                from num in words
                group num by num[0] into grp
                select new { FirstLetter = grp.Key, Words = grp };
            
            foreach (var wordgrp in wordGroups) {
                Console.WriteLine("Words that start with the letter '{0}':", wordgrp.FirstLetter);
                foreach (var word in wordgrp.Words) {
                    Console.WriteLine(word);
                }
            }
        }
        
        [Category("Grouping Operators")]
        [Title("GroupBy - Simple 3")]
        [Description("This sample uses group by to partition a list of products by Category.")]
        public void Linq42() {
            List<Product> products = GetProductList();

            var orderGroups =
                from prod in products
                group prod by prod.Category into prodGroup
                select new { Category = prodGroup.Key, Products = prodGroup };
                           
            ObjectDumper.Write(orderGroups, 1);
        }
        
        [Category("Grouping Operators")]
        [Title("GroupBy - Nested")]
        [Description("This sample uses group by to partition a list of each customer's orders, " +
                     "first by year, and then by month.")]
        public void Linq43() {
            List<Customer> customers = GetCustomerList();

            var customerOrderGroups = 
                from cust in customers
                select
                    new {cust.CompanyName, 
                         YearGroups =
                             from order in cust.Orders
                             group order by order.OrderDate.Year into yearGroup
                             select
                                 new {Year = yearGroup.Key,
                                      MonthGroups = 
                                          from order in yearGroup
                                          group order by order.OrderDate.Month into MonthGroup
                                          select new { Month = MonthGroup.Key, Orders = MonthGroup }
                                     }
                        };
                           
            ObjectDumper.Write(customerOrderGroups, 3);
        }
        
        public class AnagramEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) {
                return getCanonicalString(x) == getCanonicalString(y);
            }

            public int GetHashCode(string obj) {
                return getCanonicalString(obj).GetHashCode();
            }
            
            private string getCanonicalString(string word) {
                char[] wordChars = word.ToCharArray();
                Array.Sort<char>(wordChars);
                return new string(wordChars);
            }
        }
        
        [Category("Grouping Operators")]
        [Title("GroupBy - Comparer")]
        [Description("This sample uses GroupBy with method syntax to partition trimmed elements of an array using " +
                     "a custom comparer that matches words that are anagrams of each other.")]
        [LinkedClass("AnagramEqualityComparer")]
        public void Linq44() {
            string[] anagrams = {"from   ", " salt", " earn ", "  last   ", " near ", " form  "};

            var orderGroups = anagrams.GroupBy(w => w.Trim(), new AnagramEqualityComparer());
                           
            ObjectDumper.Write(orderGroups, 1);
        }
        
        [Category("Grouping Operators")]
        [Title("GroupBy - Comparer, Mapped")]
        [Description("This sample uses the GroupBy method to partition trimmed elements of an array using " +
                     "a custom comparer that matches words that are anagrams of each other, " +
                     "and then converts the results to uppercase.")]
        [LinkedClass("AnagramEqualityComparer")]
        public void Linq45() {
            string[] anagrams = {"from   ", " salt", " earn ", "  last   ", " near ", " form  "};

            var orderGroups = anagrams.GroupBy(
                        w => w.Trim(), 
                        a => a.ToUpper(),
                        new AnagramEqualityComparer()
                        );
                           
            ObjectDumper.Write(orderGroups, 1);
        }
                     
        [Category("Set Operators")]
        [Title("Distinct - 1")]
        [Description("This sample uses Distinct to remove duplicate elements in a sequence of " +
                    "factors of 300.")]
        public void Linq46() {
            int[] factorsOf300 = { 2, 2, 3, 5, 5 };
            
            var uniqueFactors = factorsOf300.Distinct();

            Console.WriteLine("Prime factors of 300:");
            foreach (var f in uniqueFactors) {
                Console.WriteLine(f);
            }
        }
        
        [Category("Set Operators")]
        [Title("Distinct - 2")]
        [Description("This sample uses Distinct to find the unique Category names.")]
        public void Linq47() {
            List<Product> products = GetProductList();
            
            var categoryNames = (
                from prod in products
                select prod.Category)
                .Distinct();
                                                 
            Console.WriteLine("Category names:");
            foreach (var n in categoryNames) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Set Operators")]
        [Title("Union - 1")]
        [Description("This sample uses Union to create one sequence that contains the unique values " +
                     "from both arrays.")]
        public void Linq48() {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };
            
            var uniqueNumbers = numbersA.Union(numbersB);
            
            Console.WriteLine("Unique numbers from both arrays:");
            foreach (var n in uniqueNumbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Set Operators")]
        [Title("Union - 2")]
        [Description("This sample uses the Union method to create one sequence that contains the unique first letter " +
                     "from both product and customer names. Union is only available through method syntax.")]
        public void Linq49() {
            List<Product> products = GetProductList();
            List<Customer> customers = GetCustomerList();
            
            var productFirstChars =
                from prod in products
                select prod.ProductName[0];
            var customerFirstChars =
                from cust in customers
                select cust.CompanyName[0];
            
            var uniqueFirstChars = productFirstChars.Union(customerFirstChars);
            
            Console.WriteLine("Unique first letters from Product names and Customer names:");
            foreach (var ch in uniqueFirstChars) {
                Console.WriteLine(ch);
            }
        }
        
        [Category("Set Operators")]
        [Title("Intersect - 1")]
        [Description("This sample uses Intersect to create one sequence that contains the common values " +
                    "shared by both arrays.")]
        public void Linq50() {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };
            
            var commonNumbers = numbersA.Intersect(numbersB);
            
            Console.WriteLine("Common numbers shared by both arrays:");
            foreach (var n in commonNumbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Set Operators")]
        [Title("Intersect - 2")]
        [Description("This sample uses Intersect to create one sequence that contains the common first letter " +
                     "from both product and customer names.")]
        public void Linq51() {
            List<Product> products = GetProductList();
            List<Customer> customers = GetCustomerList();
            
            var productFirstChars =
                from prod in products
                select prod.ProductName[0];
            var customerFirstChars =
                from cust in customers
                select cust.CompanyName[0];
            
            var commonFirstChars = productFirstChars.Intersect(customerFirstChars);
            
            Console.WriteLine("Common first letters from Product names and Customer names:");
            foreach (var ch in commonFirstChars) {
                Console.WriteLine(ch);
            }
        }
        
        [Category("Set Operators")]
        [Title("Except - 1")]
        [Description("This sample uses Except to create a sequence that contains the values from numbersA" +
                     "that are not also in numbersB.")]
        public void Linq52() {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };
            
            IEnumerable<int> aOnlyNumbers = numbersA.Except(numbersB);
            
            Console.WriteLine("Numbers in first array but not second array:");
            foreach (var n in aOnlyNumbers) {
                Console.WriteLine(n);
            }
        }
    
        [Category("Set Operators")]
        [Title("Except - 2")]
        [Description("This sample uses Except to create one sequence that contains the first letters " +
                     "of product names that are not also first letters of customer names.")]
        public void Linq53() {
            List<Product> products = GetProductList();
            List<Customer> customers = GetCustomerList();
            
            var productFirstChars =
                from prod in products
                select prod.ProductName[0];
            var customerFirstChars =
                from cust in customers
                select cust.CompanyName[0];
            
            var productOnlyFirstChars = productFirstChars.Except(customerFirstChars);
            
            Console.WriteLine("First letters from Product names, but not from Customer names:");
            foreach (var ch in productOnlyFirstChars) {
                Console.WriteLine(ch);
            }
        }
        
        [Category("Conversion Operators")]
        [Title("ToArray")]
        [Description("This sample uses ToArray to immediately evaluate a sequence into an array.")]
        public void Linq54() {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };
            
            var sortedDoubles =
                from d in doubles
                orderby d descending
                select d;
            var doublesArray = sortedDoubles.ToArray();
            
            Console.WriteLine("Every other double from highest to lowest:");
            for (int d = 0; d < doublesArray.Length; d += 2) {
                Console.WriteLine(doublesArray[d]);
            }             
        }
        
        [Category("Conversion Operators")]
        [Title("ToList")]
        [Description("This sample uses ToList to immediately evaluate a sequence into a List<T>.")]
        public void Linq55() {
            string[] words = { "cherry", "apple", "blueberry" };
            
            var sortedWords =
                from w in words
                orderby w
                select w;
            var wordList = sortedWords.ToList();
            
            Console.WriteLine("The sorted word list:");
            foreach (var w in wordList) {
                Console.WriteLine(w);
            }             
        }
        
        [Category("Conversion Operators")]
        [Title("ToDictionary")]
        [Description("This sample uses ToDictionary to immediately evaluate a sequence and a " +
                    "related key expression into a dictionary.")]
        public void Linq56() {
            var scoreRecords = new [] { new {Name = "Alice", Score = 50},
                                        new {Name = "Bob"  , Score = 40},
                                        new {Name = "Cathy", Score = 45}
                                    };
            
            var scoreRecordsDict = scoreRecords.ToDictionary(sr => sr.Name);
            
            Console.WriteLine("Bob's score: {0}", scoreRecordsDict["Bob"]);
        }

        [Category("Conversion Operators")]
        [Title("OfType")]
        [Description("This sample uses OfType to return only the elements of the array that are of type double.")]
        public void Linq57() {
            object[] numbers = { null, 1.0, "two", 3, "four", 5, "six", 7.0 };

            var doubles = numbers.OfType<double>();
            
            Console.WriteLine("Numbers stored as doubles:");
            foreach (var d in doubles) {
                Console.WriteLine(d);
            }       
        }

        [Category("Element Operators")]
        [Title("First - Simple")]
        [Description("This sample uses First to return the first matching element " +
                     "as a Product, instead of as a sequence containing a Product.")]
        public void Linq58() {
            List<Product> products = GetProductList();

            Product product12 = (
                from prod in products
                where prod.ProductID == 12
                select prod)
                .First();
        
            ObjectDumper.Write(product12);
        }
        
        [Category("Element Operators")]
        [Title("First - Condition")]
        [Description("This sample uses First to find the first element in the array that starts with 'o'.")]
        public void Linq59() {
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            string startsWithO = strings.First(s => s[0] == 'o');
        
            Console.WriteLine("A string starting with 'o': {0}", startsWithO);
        }
        
        [Category("Element Operators")]
        [Title("FirstOrDefault - Simple")]
        [Description("This sample uses FirstOrDefault to try to return the first element of the sequence, " +
                     "unless there are no elements, in which case the default value for that type " +
                     "is returned. FirstOrDefault is useful for creating outer joins.")]
        public void Linq61() {
            int[] numbers = {};

            int firstNumOrDefault = numbers.FirstOrDefault();
        
            Console.WriteLine(firstNumOrDefault);
        }
        
        [Category("Element Operators")]
        [Title("FirstOrDefault - Condition")]
        [Description("This sample uses FirstOrDefault to return the first product whose ProductID is 789 " +
                     "as a single Product object, unless there is no match, in which case null is returned.")]
        public void Linq62() {
            List<Product> products = GetProductList();

            Product product789 = products.FirstOrDefault(p => p.ProductID == 789);
        
            Console.WriteLine("Product 789 exists: {0}", product789 != null);
        }
             
        [Category("Element Operators")]
        [Title("ElementAt")]
        [Description("This sample uses ElementAt to retrieve the second number greater than 5 " +
                     "from an array.")]
        public void Linq64() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            int fourthLowNum = (
                from num in numbers
                where num > 5
                select num )
                .ElementAt(1);  // second number is index 1 because sequences use 0-based indexing
        
            Console.WriteLine("Second number > 5: {0}", fourthLowNum);
        }
        
        [Category("Generation Operators")]
        [Title("Range")]
        [Description("This sample uses Range to generate a sequence of numbers from 100 to 149 " +
                     "that is used to find which numbers in that range are odd and even.")]
        public void Linq65() {
            var numbers =
                from n in Enumerable.Range(100, 50)
                select new {Number = n, OddEven = n % 2 == 1 ? "odd" : "even"};
            
            foreach (var n in numbers) {
                Console.WriteLine("The number {0} is {1}.", n.Number, n.OddEven);
            }
        }
        
        [Category("Generation Operators")]
        [Title("Repeat")]
        [Description("This sample uses Repeat to generate a sequence that contains the number 7 ten times.")]
        public void Linq66() {
            var numbers = Enumerable.Repeat(7, 10);
            
            foreach (var n in numbers) {
                Console.WriteLine(n);
            }
        }
        
        
        [Category("Quantifiers")]
        [Title("Any - Simple")]
        [Description("This sample uses Any to determine if any of the words in the array " +
                     "contain the substring 'ei'.")]
        public void Linq67() {
            string[] words = { "believe", "relief", "receipt", "field" };
            
            bool iAfterE = words.Any(w => w.Contains("ei"));

            //DONE fixed typo in writeline
            Console.WriteLine("There is a word in the list that contains 'ei': {0}", iAfterE);
        }
        
        [Category("Quantifiers")]
        [Title("Any - Grouped")]
        [Description("This sample uses Any to return a grouped a list of products only for categories " +
                     "that have at least one product that is out of stock.")]
        public void Linq69() {
            List<Product> products = GetProductList();

            var productGroups =
                from prod in products
                group prod by prod.Category into prodGroup
                where prodGroup.Any(p => p.UnitsInStock == 0)
                select new { Category = prodGroup.Key, Products = prodGroup };

            ObjectDumper.Write(productGroups, 1);
        }
        
        [Category("Quantifiers")]
        [Title("All - Simple")]
        [Description("This sample uses All to determine whether an array contains " +
                     "only odd numbers.")]
        public void Linq70() {
            int[] numbers = { 1, 11, 3, 19, 41, 65, 19 };
            
            bool onlyOdd = numbers.All(n => n % 2 == 1);

            Console.WriteLine("The list contains only odd numbers: {0}", onlyOdd);
        }
                
        [Category("Quantifiers")]
        [Title("All - Grouped")]
        [Description("This sample uses All to return a grouped a list of products only for categories " +
                     "that have all of their products in stock.")]
        public void Linq72() {
            List<Product> products = GetProductList();

            var productGroups =
                from prod in products
                group prod by prod.Category into prodGroup
                where prodGroup.All(p => p.UnitsInStock > 0)
                select new { Category = prodGroup.Key, Products = prodGroup };

            ObjectDumper.Write(productGroups, 1);
        }

        [Category("Aggregate Operators")]
        [Title("Count - Simple")]
        [Description("This sample uses Count to get the number of unique prime factors of 300.")]
        public void Linq73() {
            int[] primeFactorsOf300 = { 2, 2, 3, 5, 5 };
            
            int uniqueFactors = primeFactorsOf300.Distinct().Count();

            Console.WriteLine("There are {0} unique prime factors of 300.", uniqueFactors);
        }

        [Category("Aggregate Operators")]
        [Title("Count - Conditional")]
        [Description("This sample uses Count to get the number of odd ints in the array.")]
        public void Linq74() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            int oddNumbers = numbers.Count(n => n % 2 == 1);
            
            Console.WriteLine("There are {0} odd numbers in the list.", oddNumbers);
        }

        [Category("Aggregate Operators")]
        [Title("Count - Nested")]
        [Description("This sample uses Count to return a list of customers and how many orders " +
                     "each has.")]
        public void Linq76() {
            List<Customer> customers = GetCustomerList();
            
            var orderCounts =
                from cust in customers
                select new {cust.CustomerID, OrderCount = cust.Orders.Count()};
        
            ObjectDumper.Write(orderCounts);
        }
        
        [Category("Aggregate Operators")]
        [Title("Count - Grouped")]
        [Description("This sample uses Count to return a list of categories and how many products " +
                     "each has.")]
        public void Linq77() {
            List<Product> products = GetProductList();

            var categoryCounts =
                from prod in products
                group prod by prod.Category into prodGroup
                select new {Category = prodGroup.Key, ProductCount = prodGroup.Count()};
            
            ObjectDumper.Write(categoryCounts);
        }
        
        //DONE Changed "get the total of" to "add all"
        [Category("Aggregate Operators")]
        [Title("Sum - Simple")]
        [Description("This sample uses Sum to add all the numbers in an array.")]
        public void Linq78() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            double numSum = numbers.Sum();
            
            Console.WriteLine("The sum of the numbers is {0}.", numSum);
        }

        [Category("Aggregate Operators")]
        [Title("Sum - Projection")]
        [Description("This sample uses Sum to get the total number of characters of all words " +
                     "in the array.")]
        public void Linq79() {
            string[] words = { "cherry", "apple", "blueberry" };
            
            double totalChars = words.Sum(w => w.Length);
            
            Console.WriteLine("There are a total of {0} characters in these words.", totalChars);
        }

        
        
        [Category("Aggregate Operators")]
        [Title("Sum - Grouped")]
        [Description("This sample uses Sum to get the total units in stock for each product Category.")]
        public void Linq80() {
            List<Product> products = GetProductList();

            var categories =
                from prod in products
                group prod by prod.Category into prodGroup
                select new {Category = prodGroup.Key, TotalUnitsInStock = prodGroup.Sum(p => p.UnitsInStock)};
        
            ObjectDumper.Write(categories);
        }
        
        [Category("Aggregate Operators")]
        [Title("Min - Simple")]
        [Description("This sample uses Min to get the lowest number in an array.")]
        public void Linq81() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            int minNum = numbers.Min();
            
            Console.WriteLine("The minimum number is {0}.", minNum);
        }

        [Category("Aggregate Operators")]
        [Title("Min - Projection")]
        [Description("This sample uses Min to get the length of the shortest word in an array.")]
        public void Linq82() {
            string[] words = { "cherry", "apple", "blueberry" };
            
            int shortestWord = words.Min(w => w.Length);
            
            Console.WriteLine("The shortest word is {0} characters long.", shortestWord);
        }
        
        [Category("Aggregate Operators")]
        [Title("Min - Grouped")]
        [Description("This sample uses Min to get the cheapest price among each Category's products.")]
        public void Linq83() {
            List<Product> products = GetProductList();

            var categories =
                from prod in products
                group prod by prod.Category into prodGroup
                select new {Category = prodGroup.Key, CheapestPrice = prodGroup.Min(p => p.UnitPrice)};
        
            ObjectDumper.Write(categories);
        }
        
        [Category("Aggregate Operators")]
        [Title("Min - Elements")]
        [Description("This sample uses Min to get the products with the lowest price in each Category.")]
        public void Linq84() {
            List<Product> products = GetProductList();

            var categories =
                from prod in products
                group prod by prod.Category into prodGroup
                let minPrice = prodGroup.Min(p => p.UnitPrice)
                select new {Category = prodGroup.Key, CheapestProducts = prodGroup.Where(p => p.UnitPrice == minPrice)};
        
            ObjectDumper.Write(categories, 1);
        }
        
        [Category("Aggregate Operators")]
        [Title("Max - Simple")]
        [Description("This sample uses Max to get the highest number in an array. Note that the method returns a single value.")]
        public void Linq85() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            int maxNum = numbers.Max();
            
            Console.WriteLine("The maximum number is {0}.", maxNum);
        }

        [Category("Aggregate Operators")]
        [Title("Max - Projection")]
        [Description("This sample uses Max to get the length of the longest word in an array.")]
        public void Linq86() {
            string[] words = { "cherry", "apple", "blueberry" };
            
            int longestLength = words.Max(w => w.Length);
            
            Console.WriteLine("The longest word is {0} characters long.", longestLength);
        }
        
        [Category("Aggregate Operators")]
        [Title("Max - Grouped")]
        [Description("This sample uses Max to get the most expensive price among each Category's products.")]
        public void Linq87() {
            List<Product> products = GetProductList();

            var categories =
                from prod in products
                group prod by prod.Category into prodGroup
                select new {Category = prodGroup.Key, MostExpensivePrice = prodGroup.Max(p => p.UnitPrice)};
        
            ObjectDumper.Write(categories);
        }
        
        [Category("Aggregate Operators")]
        [Title("Max - Elements")]
        [Description("This sample uses Max to get the products with the most expensive price in each Category.")]
        public void Linq88() {
            List<Product> products = GetProductList();

            var categories =
                from prod in products
                group prod by prod.Category into prodGroup
                let maxPrice = prodGroup.Max(p => p.UnitPrice)
                select new {Category = prodGroup.Key, MostExpensiveProducts = prodGroup.Where(p => p.UnitPrice == maxPrice)};
        
            ObjectDumper.Write(categories, 1);
        }
        
        [Category("Aggregate Operators")]
        [Title("Average - Simple")]
        [Description("This sample uses Average to get the average of all numbers in an array.")]
        public void Linq89() {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            double averageNum = numbers.Average();
            
            Console.WriteLine("The average number is {0}.", averageNum);
        }

        [Category("Aggregate Operators")]
        [Title("Average - Projection")]
        [Description("This sample uses Average to get the average length of the words in the array.")]
        public void Linq90() {
            string[] words = { "cherry", "apple", "blueberry" };
            
            double averageLength = words.Average(w => w.Length);
            
            Console.WriteLine("The average word length is {0} characters.", averageLength);
        }
        
        [Category("Aggregate Operators")]
        [Title("Average - Grouped")]
        [Description("This sample uses Average to get the average price of each Category's products.")]
        public void Linq91() {
            List<Product> products = GetProductList();

            var categories =
                from prod in products
                group prod by prod.Category into prodGroup
                select new {Category = prodGroup.Key, AveragePrice = prodGroup.Average(p => p.UnitPrice)};
        
            ObjectDumper.Write(categories);
        }

        [Category("Aggregate Operators")]
        [Title("Aggregate - Simple")]
        [Description("This sample uses Aggregate to create a running product on the array that " +
                     "calculates the total product of all elements.")]
        public void Linq92() {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };
            
            double product = doubles.Aggregate((runningProduct, nextFactor) => runningProduct * nextFactor);
            
            Console.WriteLine("Total product of all numbers: {0}", product);
        }
        
        [Category("Aggregate Operators")]
        [Title("Aggregate - Seed")]
        [Description("This sample uses Aggregate to create a running account balance that " +
                     "subtracts each withdrawal from the initial balance of 100, as long as " +
                     "the balance never drops below 0.")]
        public void Linq93() {
            double startBalance = 100.0;
            
            int[] attemptedWithdrawals = { 20, 10, 40, 50, 10, 70, 30 };
            
            double endBalance = 
                attemptedWithdrawals.Aggregate(startBalance,
                    (balance, nextWithdrawal) =>
                        ( (nextWithdrawal <= balance) ? (balance - nextWithdrawal) : balance ) );
            
            Console.WriteLine("Ending balance: {0}", endBalance);
        }
        
        [Category("Miscellaneous Operators")]
        [Title("Concat - 1")]
        [Description("This sample uses Concat to create one sequence that contains each array's " +
                     "values, one after the other.")]
        public void Linq94() {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };
            
            var allNumbers = numbersA.Concat(numbersB);
            
            Console.WriteLine("All numbers from both arrays:");
            foreach (var n in allNumbers) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Miscellaneous Operators")]
        [Title("Concat - 2")]
        [Description("This sample uses Concat to create one sequence that contains the names of " +
                     "all customers and products, including any duplicates.")]
        public void Linq95() {
            List<Customer> customers = GetCustomerList();
            List<Product> products = GetProductList();
            
            var customerNames =
                from cust in customers
                select cust.CompanyName;
            var productNames =
                from prod in products
                select prod.ProductName;
            
            var allNames = customerNames.Concat(productNames);
            
            Console.WriteLine("Customer and product names:");
            foreach (var n in allNames) {
                Console.WriteLine(n);
            }
        }
        
        [Category("Miscellaneous Operators")]
        [Title("EqualAll - 1")]
        [Description("This sample uses SequenceEquals to see if two sequences match on all elements " +
                     "in the same order.")]
        public void Linq96() {
            var wordsA = new string[] { "cherry", "apple", "blueberry" };
            var wordsB = new string[] { "cherry", "apple", "blueberry" };
            
            bool match = wordsA.SequenceEqual(wordsB);
            
            Console.WriteLine("The sequences match: {0}", match);
        }
        
        [Category("Miscellaneous Operators")]
        [Title("EqualAll - 2")]
        [Description("This sample uses SequenceEqual to see if two sequences match on all elements " +
                     "in the same order.")]
        public void Linq97() {
            var wordsA = new string[] { "cherry", "apple", "blueberry" };
            var wordsB = new string[] { "apple", "blueberry", "cherry" };

            bool match = wordsA.SequenceEqual(wordsB);
            
            Console.WriteLine("The sequences match: {0}", match);
        }

        [Category("Query Execution")]
        [Title("Deferred Execution")]
        [Description("The following sample shows how query execution is deferred until the query is " +
                     "enumerated at a foreach statement.")]
        public void Linq99() {
            
            // Queries are not executed until you enumerate over them.
            int[] numbers = new int[] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            
            int i = 0;
            var simpleQuery =
                from num in numbers
                select ++i;

            // The local variable 'i' is not incremented
            // until the query is executed in the foreach loop.
            Console.WriteLine("The current value of i is {0}", i); //i is still zero

            foreach (var item in simpleQuery) {
                Console.WriteLine("v = {0}, i = {1}", item, i); // now i is incremented          
            }  
        }
        
        [Category("Query Execution")]
        [Title("Immediate Execution")]
        [Description("The following sample shows how queries can be executed immediately, and their results " +
                    " stored in memory, with methods such as ToList.")]
        public void Linq100() {

            // Methods like ToList(), Max(), and Count() cause the query to be
            // executed immediately.            
            int[] numbers = new int[] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };        
            
            int i = 0;
            var immediateQuery = (
                from num in numbers
                select ++i )
                .ToList();

            Console.WriteLine("The current value of i is {0}", i); //i has been incremented
            
            foreach (var item in immediateQuery) {
                Console.WriteLine("v = {0}, i = {1}", item, i);
            }  
        }

        [Category("Query Execution")]
        [Title("Query Reuse")]
        [Description("The following sample shows how, because of deferred execution, queries can be used " +
                     "again after data changes and will then operate on the new data.")]
        public void Linq101() {

            // Deferred execution lets us define a query once
            // and then reuse it later in various ways.
            int[] numbers = new int[] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            var lowNumbers =
                from num in numbers
                where num <= 3
                select num;

            Console.WriteLine("First run numbers <= 3:");
            foreach (int n in lowNumbers) {
                Console.WriteLine(n);
            }

            // Query the original query.
            var lowEvenNumbers =
                from num in lowNumbers
                where num % 2 == 0
                select num;

            Console.WriteLine("Run lowEvenNumbers query:");
            foreach (int n in lowEvenNumbers)
            {
                Console.WriteLine(n);
            }

            // Modify the source data.
            for (int i = 0; i < 10; i++) {
                numbers[i] = -numbers[i];
            }

            // During this second run, the same query object,
            // lowNumbers, will be iterating over the new state
            // of numbers[], producing different results:
            Console.WriteLine("Second run numbers <= 3:");
            foreach (int n in lowNumbers) {
                Console.WriteLine(n);
            }
        }

        [Category("Join Operators")]
        [Title("Inner Join")]
        [Description("This sample shows how to perform a simple inner equijoin of two sequences to " +
            "to produce a flat result set that consists of each element in suppliers that has a matching element " +
            "in customers.")]
        public void Linq102() {

            List<Customer> customers = GetCustomerList();
            List<Supplier> suppliers = GetSupplierList();

            var custSupJoin = 
                from sup in suppliers
                join cust in customers on sup.Country equals cust.Country
                select new { Country = sup.Country, SupplierName = sup.SupplierName, CustomerName = cust.CompanyName };

            foreach (var item in custSupJoin) {
                Console.WriteLine("Country = {0}, Supplier = {1}, Customer = {2}", item.Country, item.SupplierName, item.CustomerName);
            }
        }      

        [Category("Join Operators")]
        [Title("Group Join")]
        [Description("A group join produces a hierarchical sequence. The following query is an inner join " +
                    " that produces a sequence of objects, each of which has a key and an inner sequence of all matching elements.")]
        public void Linq103() {
            
               
            List<Customer> customers = GetCustomerList();
            List<Supplier> suppliers = GetSupplierList();

            var custSupQuery =
                from sup in suppliers
                join cust in customers on sup.Country equals cust.Country into cs
                select new { Key = sup.Country, Items = cs };

           
            foreach (var item in custSupQuery) {
                Console.WriteLine(item.Key + ":");
                foreach (var element in item.Items) 
                {
                    Console.WriteLine("   " + element.CompanyName); 
                }
            }
        }

        [Category("Join Operators")]
        [Title("Cross Join with Group Join")]
        [Description("The group join operator is more general than join, as this slightly more verbose " +
            "version of the cross join sample shows.")]
        public void Linq104() {
            string[] categories = new string[]{ 
                "Beverages", 
                "Condiments", 
                "Vegetables", 
                "Dairy Products", 
                "Seafood" };
               
            List<Product> products = GetProductList();

            var prodByCategory = 
                from cat in categories
                join prod in products on cat equals prod.Category into ps
                from p in ps   
                select new { Category = cat, p.ProductName };

            foreach (var item in prodByCategory) {
                Console.WriteLine(item.ProductName + ": " + item.Category);
            }
        }
        [Category("Join Operators")]
        [Title("Left Outer Join")]
        [Description("A left outer join produces a result set that includes all the left hand side elements at " + 
            "least once, even if they don't match any right hand side elements.")]
        public void Linq105() {
            List<Customer> customers = GetCustomerList();
            List<Supplier> suppliers = GetSupplierList();

            var supplierCusts = 
                from sup in suppliers
                join cust in customers on sup.Country equals cust.Country into cs
                from c in cs.DefaultIfEmpty()  // DefaultIfEmpty preserves left-hand elements that have no matches on the right side 
                orderby sup.SupplierName
                select new { Country = sup.Country, CompanyName = c == null ? "(No customers)" : c.CompanyName,
                             SupplierName = sup.SupplierName};

            foreach (var item in supplierCusts) {
                Console.WriteLine("{0} ({1}): {2}", item.SupplierName, item.Country, item.CompanyName);
            }
        }

        [Category("Join Operators")]
        [Title("Left Outer Join No. 2")]
        [Description("For each customer in the table of customers, this query returns all the suppliers " +
                     "from that same country, or else a string indicating that no suppliers from that country were found.")]
        public void Linq106()
        {

            List<Customer> customers = GetCustomerList();
            List<Supplier> suppliers = GetSupplierList();

            var custSuppliers =
                from cust in customers
                join sup in suppliers on cust.Country equals sup.Country into ss
                from s in ss.DefaultIfEmpty()
                orderby cust.CompanyName
                select new { Country = cust.Country, CompanyName = cust.CompanyName,
                             SupplierName = s == null ? "(No suppliers)" : s.SupplierName };

            foreach (var item in custSuppliers) {
                Console.WriteLine("{0} ({1}): {2}", item.CompanyName, item.Country, item.SupplierName);
            }
        }

        [Category("Join Operators")]
        [Title("Left Outer Join with Composite Key")]
        [Description("For each supplier in the table of suppliers, this query returns all the customers " +
                     "from the same city and country, or else a string indicating that no customers from that city/country were found. " +
                     "Note the use of anonymous types to encapsulate the multiple key values.")]
        public void Linq107()
        {
            List<Customer> customers = GetCustomerList();
            List<Supplier> suppliers = GetSupplierList();

            var supplierCusts =
                from sup in suppliers
                join cust in customers on new { sup.City, sup.Country } equals new { cust.City, cust.Country } into cs
                from c in cs.DefaultIfEmpty() //Remove DefaultIfEmpty method call to make this an inner join
                orderby sup.SupplierName
                select new { Country = sup.Country, 
                             City = sup.City,
                             SupplierName = sup.SupplierName,
                             CompanyName = c == null ? "(No customers)" : c.CompanyName
                           };

            foreach (var item in supplierCusts)
            {
                Console.WriteLine("{0} ({1}, {2}): {3}", item.SupplierName, item.City, item.Country, item.CompanyName);
            }
        }

        [Category("* Sample Data *")]
        [Title("CustomerList / ProductList / Supplier List")]
        [Description("This method displays the sample data used by the queries above.  You can also see " +
                     "the method below that constructs the lists.  ProductList and supplierList are built by using " +
                     "collection initializers and CustomerList uses XLinq to read its values " +
                     "into memory from an XML document.")]
        [LinkedMethod("GetProductList")]
        [LinkedMethod("GetSupplierList")]
        [LinkedMethod("GetCustomerList")]
        [LinkedMethod("createLists")]
        public void Linq115() {
            ObjectDumper.Write(GetCustomerList(), 1);

            Console.WriteLine();

            ObjectDumper.Write(GetProductList());
        }

        [Category("Custom")]
        [Title("nazwy produktów, które są na stanie, kosztują mniej niż 10 i należą do kategorii Seafood")]
        [Description("nazwy produktów, które są na stanie, kosztują mniej niż 10 i należą do kategorii Seafood")]
        public void Linq116()
        {
            List<Product> products = GetProductList();

            var result = products
                .Where(p => p.Category == "Seafood" && p.UnitPrice < 10 && p.UnitsInStock > 0)
                .Select(p => new
                {
                    name = p.ProductName,
                    category = p.Category,
                    unitPrice = p.UnitPrice,
                    inStock = p.UnitsInStock
                });

            foreach (var item in result)
            {
                Console.WriteLine("Product {0}, | Category {1}, | Price {2}, | InStock {3})", item.name, item.category, item.unitPrice, item.inStock);
            }
        }

        [Category("Custom")]
        [Title("Najtańsze i najdroższe produkty wg kategorii")]
        [Description("Najtańsze i najdroższe produkty wg kategorii")]
        public void Linq117()
        {
            List<Product> products = GetProductList();

            var result = products.OrderBy(p => p.UnitPrice)
                .GroupBy(p => p.Category)
                .Select(p => new {
                    category = p.Key,
                    minPrice = p.First().UnitPrice,
                    minProduct = p.First().ProductName,
                    maxPrice = p.Last().UnitPrice,
                    maxProduct = p.Last().ProductName

                });                

            foreach (var item in result)
            {
                Console.WriteLine("Category {0}, (Min: {1}, {2}), (Max: {3}, {4})", item.category, item.minProduct, item.minPrice, item.maxProduct, item.maxPrice);
            }
        }

        [Category("Custom")]
        [Title("Cenę, dla której jest najwięcej sztuk produktów")]
        [Description("Cenę, dla której jest najwięcej sztuk produktów")]
        public void Linq118()
        {
            List<Product> products = GetProductList();

            var result = products
                .GroupBy(p => p.UnitPrice)
                .Select(p => new
                {
                    price = p.Key,
                    units = p.Sum(x => x.UnitsInStock)
                })
                .OrderByDescending(p => p.units)
                .Take(1);

            Console.WriteLine("{0}", result);
        }

        [Category("Custom")]
        [Title("Dla każdego produktu podać liczbę produktów, które są od niego tańsze lub jest ich mniej sztuk na składzie")]
        [Description("Dla każdego produktu podać liczbę produktów, które są od niego tańsze lub jest ich mniej sztuk na składzie")]
        public void Linq119()
        {
            List<Product> products = GetProductList();

            var result = productList.Select(product => new
            {
                product = product,
                counter = productList.Where(product2 => product.UnitPrice < product2.UnitPrice || product.UnitsInStock > product2.UnitsInStock).Count()
            });


            foreach (var item in result)
            {
                Console.WriteLine("Produkt: {0}, ilosc innych produktow tanszych lub o mniejszej ilosci sztuk: {1}", item.product.ProductName, item.counter);
            }
        }

        [Category("Custom")]
        [Title("Dla każdego produktu podaj liczbę produktów, które kosztują tyle samo")]
        [Description("Dla każdego produktu podaj liczbę produktów, które kosztują tyle samo")]
        public void Linq120()
        {
            List<Product> productList = GetProductList();
            var result = productList.Select(product => new
            {
                product = product,
                counter = productList.Where(product2 => product.UnitPrice == product2.UnitPrice).Count()
            });

            foreach (var item in result)
            {
                Console.WriteLine("Produkt {0}, ilosc produktow o takiej samej cenie: {1}", item.product.ProductName, item.counter);
            }

        }

        [Category("Custom")]
        [Title("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura - WYKLAD")]
        [Description("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura - WYKLAD")]
        public void Linq121()
        {
            List<Product> productList = GetProductList();
            var result = productList
                .Where(product => productList
                   .Where(product2 => product2.ProductName == "Ikura")
                   .Select(product2 => product2.UnitPrice).Contains(product.UnitPrice)
                ).Select(product => product.ProductName);


            foreach (var item in result)
            {
                Console.WriteLine("Produkt: {0}", item);
            }

        }

        [Category("Custom")]
        [Title("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura")]
        [Description("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura")]
        public void Linq122()
        {
            List<Product> productList = GetProductList();

            var ikuraPrice = productList.Where(product2 => product2.ProductName == "Ikura").Select(product2 => product2.UnitPrice).ToList();

            var result = productList
                .Where(product => ikuraPrice.Contains(product.UnitPrice))
                .Select(product => product.ProductName);

            foreach (var item in result)
            {
                Console.WriteLine("Produkt: {0}", item);
            }

        }

        [Category("Benchmark")]
        [Title("nazwy produktów, które są na stanie, kosztują mniej niż 10 i należą do kategorii Seafood")]
        [Description("nazwy produktów, które są na stanie, kosztują mniej niż 10 i należą do kategorii Seafood")]
        public void Linq123()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => products
                .Where(p => p.Category == "Seafood" && p.UnitPrice < 10 && p.UnitsInStock > 0)
                .Select(p => new
                {
                    name = p.ProductName,
                    category = p.Category,
                    unitPrice = p.UnitPrice,
                    inStock = p.UnitsInStock
                });

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);

        }

        [Category("Benchmark")]
        [Title("Najtańsze i najdroższe produkty wg kategorii")]
        [Description("Najtańsze i najdroższe produkty wg kategorii")]
        public void Linq124()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => products.OrderBy(p => p.UnitPrice)
                .GroupBy(p => p.Category)
                .Select(p => new
                {
                    category = p.Key,
                    minPrice = p.First().UnitPrice,
                    minProduct = p.First().ProductName,
                    maxPrice = p.Last().UnitPrice,
                    maxProduct = p.Last().ProductName

                });

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);
        }

        [Category("Benchmark")]
        [Title("Cenę, dla której jest najwięcej sztuk produktów")]
        [Description("Cenę, dla której jest najwięcej sztuk produktów")]
        public void Linq125()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => products
                .GroupBy(p => p.UnitPrice)
                .Select(p => new
                {
                    price = p.Key,
                    units = p.Sum(x => x.UnitsInStock)
                })
                .OrderByDescending(p => p.units)
                .Take(1);

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);
        }

        [Category("Benchmark")]
        [Title("Dla każdego produktu podać liczbę produktów, które są od niego tańsze lub jest ich mniej sztuk na składzie")]
        [Description("Dla każdego produktu podać liczbę produktów, które są od niego tańsze lub jest ich mniej sztuk na składzie")]
        public void Linq126()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => productList.Select(product => new
            {
                product = product,
                counter = productList.Where(product2 => product.UnitPrice < product2.UnitPrice || product.UnitsInStock > product2.UnitsInStock).Count()
            });


            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);
        }

        [Category("Benchmark")]
        [Title("Dla każdego produktu podaj liczbę produktów, które kosztują tyle samo")]
        [Description("Dla każdego produktu podaj liczbę produktów, które kosztują tyle samo")]
        public void Linq127()
        {
            List<Product> productList = GetProductList();
            Func<IEnumerable<dynamic>> func = () => productList.Select(product => new
            {
                product = product,
                counter = productList.Where(product2 => product.UnitPrice == product2.UnitPrice).Count()
            });

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);

        }

        [Category("Benchmark")]
        [Title("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura - WYKLAD")]
        [Description("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura - WYKLAD")]
        public void Linq128()
        {
            List<Product> productList = GetProductList();
            Func<IEnumerable<dynamic>> func = () => productList
                .Where(product => productList
                   .Where(product2 => product2.ProductName == "Ikura")
                   .Select(product2 => product2.UnitPrice).Contains(product.UnitPrice)
                ).Select(product => product.ProductName);


            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);

        }

        [Category("Benchmark")]
        [Title("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura")]
        [Description("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura")]
        public void Linq129()
        {
            List<Product> productList = GetProductList();
            Func<IEnumerable<dynamic>> func = () =>
            {
                var ikuraPrice = productList.Where(product2 => product2.ProductName == "Ikura").Select(product2 => product2.UnitPrice).ToList();

                return productList
                    .Where(product => ikuraPrice.Contains(product.UnitPrice))
                    .Select(product => product.ProductName);

            };
            double time = QuerySamples.Benchmark.getTimeOf(func);
            Console.WriteLine("Time {0}", time);

        }

        [Category("Parallel")]
        [Title("nazwy produktów, które są na stanie, kosztują mniej niż 10 i należą do kategorii Seafood")]
        [Description("nazwy produktów, które są na stanie, kosztują mniej niż 10 i należą do kategorii Seafood")]
        public void Linq130()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => products.AsParallel()
                .Where(p => p.Category == "Seafood" && p.UnitPrice < 10 && p.UnitsInStock > 0)
                .Select(p => new
                {
                    name = p.ProductName,
                    category = p.Category,
                    unitPrice = p.UnitPrice,
                    inStock = p.UnitsInStock
                });

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);

        }

        [Category("Parallel")]
        [Title("Najtańsze i najdroższe produkty wg kategorii")]
        [Description("Najtańsze i najdroższe produkty wg kategorii")]
        public void Linq131()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => products.AsParallel()
                .OrderBy(p => p.UnitPrice)
                .GroupBy(p => p.Category)
                .Select(p => new
                {
                    category = p.Key,
                    minPrice = p.First().UnitPrice,
                    minProduct = p.First().ProductName,
                    maxPrice = p.Last().UnitPrice,
                    maxProduct = p.Last().ProductName

                });

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);
        }

        [Category("Parallel")]
        [Title("Cenę, dla której jest najwięcej sztuk produktów")]
        [Description("Cenę, dla której jest najwięcej sztuk produktów")]
        public void Linq132()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => products.AsParallel()
                .GroupBy(p => p.UnitPrice)
                .Select(p => new
                {
                    price = p.Key,
                    units = p.Sum(x => x.UnitsInStock)
                })
                .OrderByDescending(p => p.units)
                .Take(1);

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);
        }

        [Category("Parallel")]
        [Title("Dla każdego produktu podać liczbę produktów, które są od niego tańsze lub jest ich mniej sztuk na składzie")]
        [Description("Dla każdego produktu podać liczbę produktów, które są od niego tańsze lub jest ich mniej sztuk na składzie")]
        public void Linq133()
        {
            List<Product> products = GetProductList();

            Func<IEnumerable<dynamic>> func = () => productList.AsParallel()
            .Select(product => new
            {
                product = product,
                counter = productList.Where(product2 => product.UnitPrice < product2.UnitPrice || product.UnitsInStock > product2.UnitsInStock).Count()
            });


            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);
        }

        [Category("Parallel")]
        [Title("Dla każdego produktu podaj liczbę produktów, które kosztują tyle samo")]
        [Description("Dla każdego produktu podaj liczbę produktów, które kosztują tyle samo")]
        public void Linq134()
        {
            List<Product> productList = GetProductList();
            Func<IEnumerable<dynamic>> func = () => productList.AsParallel()
            .Select(product => new
            {
                product = product,
                counter = productList.Where(product2 => product.UnitPrice == product2.UnitPrice).Count()
            });

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);

        }

        [Category("Parallel")]
        [Title("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura - WYKLAD")]
        [Description("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura - WYKLAD")]
        public void Linq135()
        {
            List<Product> productList = GetProductList();
            Func<IEnumerable<dynamic>> func = () => productList
                .AsParallel()
                .Where(product => productList
                   .Where(product2 => product2.ProductName == "Ikura")
                   .Select(product2 => product2.UnitPrice).Contains(product.UnitPrice)
                ).Select(product => product.ProductName);


            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);

        }

        [Category("Parallel")]
        [Title("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura")]
        [Description("produkty, których cena jednostkowa, jest równa cenie produktu o nazwie Ikura")]
        public void Linq136()
        {
            List<Product> productList = GetProductList();
            Func<IEnumerable<dynamic>> func = () =>
            {
                var ikuraPrice = productList.Where(product2 => product2.ProductName == "Ikura").Select(product2 => product2.UnitPrice).ToList();

                return productList
                    .Where(product => ikuraPrice.Contains(product.UnitPrice))
                    .Select(product => product.ProductName);
            };

            double time = QuerySamples.Benchmark.getAverageTime(func);
            Console.WriteLine("Time {0}", time);

        }

        public List<Product> GetProductList() {
            if (productList == null)
                createLists();
            
            return productList;
        }

        public List<Supplier> GetSupplierList()
        {
            if (supplierList == null)
                createLists();

            return supplierList;
        }

        public List<Customer> GetCustomerList() {
            if (customerList == null)
                createLists();
            
            return customerList;
        }

        private void createLists() {
            // Product data created in-memory using collection initializer:
            ICollection<Product> productsCollection = new List<Product>();
            Data.fillProducts(ref productsCollection);

            productList = (List<Product>)productsCollection;

            supplierList = new List<Supplier>(){
                    new Supplier {SupplierName = "Exotic Liquids", Address = "49 Gilbert St.", City = "London", Country = "UK"},
                    new Supplier {SupplierName = "New Orleans Cajun Delights", Address = "P.O. Box 78934", City = "New Orleans", Country = "USA"},
                    new Supplier {SupplierName = "Grandma Kelly's Homestead", Address = "707 Oxford Rd.", City = "Ann Arbor", Country = "USA"},
                    new Supplier {SupplierName = "Tokyo Traders", Address = "9-8 Sekimai Musashino-shi", City = "Tokyo", Country = "Japan"},
                    new Supplier {SupplierName = "Cooperativa de Quesos 'Las Cabras'", Address = "Calle del Rosal 4", City = "Oviedo", Country = "Spain"},
                    new Supplier {SupplierName = "Mayumi's", Address = "92 Setsuko Chuo-ku", City = "Osaka", Country = "Japan"},
                    new Supplier {SupplierName = "Pavlova, Ltd.", Address = "74 Rose St. Moonie Ponds", City = "Melbourne", Country = "Australia"},
                    new Supplier {SupplierName = "Specialty Biscuits, Ltd.", Address = "29 King's Way", City = "Manchester", Country = "UK"},
                    new Supplier {SupplierName = "PB Knäckebröd AB", Address = "Kaloadagatan 13", City = "Göteborg", Country = "Sweden"},
                    new Supplier {SupplierName = "Refrescos Americanas LTDA", Address = "Av. das Americanas 12.890", City = "Sao Paulo", Country = "Brazil"},
                    new Supplier {SupplierName = "Heli Süßwaren GmbH & Co. KG", Address = "Tiergartenstraße 5", City = "Berlin", Country = "Germany"},
                    new Supplier {SupplierName = "Plutzer Lebensmittelgroßmärkte AG", Address = "Bogenallee 51", City = "Frankfurt", Country = "Germany"},
                    new Supplier {SupplierName = "Nord-Ost-Fisch Handelsgesellschaft mbH", Address = "Frahmredder 112a", City = "Cuxhaven", Country = "Germany"},
                    new Supplier {SupplierName = "Formaggi Fortini s.r.l.", Address = "Viale Dante, 75", City = "Ravenna", Country = "Italy"},
                    new Supplier {SupplierName = "Norske Meierier", Address = "Hatlevegen 5", City = "Sandvika", Country = "Norway"},
                    new Supplier {SupplierName = "Bigfoot Breweries", Address = "3400 - 8th Avenue Suite 210", City = "Bend", Country = "USA"},
                    new Supplier {SupplierName = "Svensk Sjöföda AB", Address = "Brovallavägen 231", City = "Stockholm", Country = "Sweden"},
                    new Supplier {SupplierName = "Aux joyeux ecclésiastiques", Address = "203, Rue des Francs-Bourgeois", City = "Paris", Country = "France"},
                    new Supplier {SupplierName = "New England Seafood Cannery", Address = "Order Processing Dept. 2100 Paul Revere Blvd.", City = "Boston", Country = "USA"},
                    new Supplier {SupplierName = "Leka Trading", Address = "471 Serangoon Loop, Suite #402", City = "Singapore", Country = "Singapore"},
                    new Supplier {SupplierName = "Lyngbysild", Address = "Lyngbysild Fiskebakken 10", City = "Lyngby", Country = "Denmark"},
                    new Supplier {SupplierName = "Zaanse Snoepfabriek", Address = "Verkoop Rijnweg 22", City = "Zaandam", Country = "Netherlands"},
                    new Supplier {SupplierName = "Karkki Oy", Address = "Valtakatu 12", City = "Lappeenranta", Country = "Finland"},
                    new Supplier {SupplierName = "G'day, Mate", Address = "170 Prince Edward Parade Hunter's Hill", City = "Sydney", Country = "Australia"},
                    new Supplier {SupplierName = "Ma Maison", Address = "2960 Rue St. Laurent", City = "Montréal", Country = "Canada"},
                    new Supplier {SupplierName = "Pasta Buttini s.r.l.", Address = "Via dei Gelsomini, 153", City = "Salerno", Country = "Italy"},
                    new Supplier {SupplierName = "Escargots Nouveaux", Address = "22, rue H. Voiron", City = "Montceau", Country = "France"},
                    new Supplier {SupplierName = "Gai pâturage", Address = "Bat. B 3, rue des Alpes", City = "Annecy", Country = "France"},
                    new Supplier {SupplierName = "Forêts d'érables", Address = "148 rue Chasseur", City = "Ste-Hyacinthe", Country = "Canada"},
                };           
        
        
            // Customer/order data read into memory from XML file using XLinq:
            string customerListPath = Path.GetFullPath(Path.Combine(dataPath, "customers.xml"));

            customerList = (
                from e in XDocument.Load(customerListPath).
                          Root.Elements("customer")
                select new Customer {
                    CustomerID = (string)e.Element("id"),
                    CompanyName = (string)e.Element("name"),
                    Address = (string)e.Element("address"),
                    City = (string)e.Element("city"),
                    Region = (string)e.Element("region"),
                    PostalCode = (string)e.Element("postalcode"),
                    Country = (string)e.Element("country"),
                    Phone = (string)e.Element("phone"),
                    Fax = (string)e.Element("fax"),
                    Orders = (
                        from o in e.Elements("orders").Elements("order")
                        select new Order {
                            OrderID = (int)o.Element("id"),
                            OrderDate = (DateTime)o.Element("orderdate"),
                            Total = (decimal)o.Element("total") } )
                        .ToArray() } )
                .ToList();
        }
    }
}
