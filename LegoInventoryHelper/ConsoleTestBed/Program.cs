using BricklinkSharp.Client;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using LegoInventoryHelper;
using LegoInventoryHelper.DatabaseContext;
using LegoInventoryHelper.ExcelImport;
using LegoInventoryHelper.Models;
using Microsoft.Extensions.Configuration;
using ThridPartyServiceAccessor;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var brickLink = new BrickLinkAccessor(configuration);
var rebrickable = new RebrickableAccessor(configuration);
var legoSetDataRetriver = new LegoSetDataRetriver(brickLink, rebrickable);
var legoInventoryContext = new LegoInventoryContext(configuration);
var crud = new CRUD(legoSetDataRetriver, legoInventoryContext);

var allItems = (await crud.ReadAllLegoInventoryItems()).Payload;
var sumPricesBought = allItems.Sum(x => x.PriceBought);
var itemsWithoutPrice = allItems.Where(x => x.Prices.Count < 1);

var averagePrice = allItems.Average(x => x.PriceBought);
var currentValue = allItems.Sum(x => x.Prices.FirstOrDefault()?.QuantityAveragePrice);

//var all = await crud.ReadAllLegoInventoryItems();

//foreach (var item in all.Payload)
//{
//    _ = await crud.Delete(item.ID);
//}

//var a = await crud.CreateLegoInventoryItem(new CreateInventoryItem("75275", 199.99));
//var b = await crud.Delete(a.Payload.ID);

//var itemsFromExcelFile = ExcelImport.ExtractSetsFromExcel(@"C:\Users\Marvin\OneDrive\Desktop\Inventar\Lego_Inventar.xlsx");
//foreach (var createInventoryItem in itemsFromExcelFile)
//{
//    var legoSet = (await crud.CreateLegoInventoryItem(createInventoryItem)).Payload;
//    if (legoSet == null) continue;
//    var prices = legoSet.Prices.OrderByDescending(x => x.RequestDate).First();
//    Console.WriteLine($"{legoSet.Name} : {legoSet.SetID} : {legoSet.PriceBought} : {prices.MinPrice} : {prices.QuantityAveragePrice - legoSet.PriceBought}");
//    Thread.Sleep(1000);
//}
//while(true)
//{
//    Console.WriteLine("SetID eingeben");
//    var setID = Console.ReadLine();
//    Console.WriteLine("Preis eingeben");
//    var price = double.Parse(Console.ReadLine());
//    var createInventoryItem = new CreateInventoryItem(setID, price);
//    var legoSet = await crud.CreateLegoInventoryItem(createInventoryItem);
//    var prices = legoSet.Prices.OrderByDescending(x => x.RequestDate).First();
//    Console.WriteLine($"{legoSet.Name} : {legoSet.SetID} : {legoSet.PriceBought} : {prices.QuantityAveragePrice} : {prices.QuantityAveragePrice-legoSet.PriceBought}");
//}

Console.ReadLine();