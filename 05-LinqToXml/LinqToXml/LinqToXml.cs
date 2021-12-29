using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace LinqToXml
{
    public static class LinqToXml
    {
        /// <summary>
        /// Creates hierarchical data grouped by category
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation (refer to CreateHierarchySourceFile.xml in Resources)</param>
        /// <returns>Xml representation (refer to CreateHierarchyResultFile.xml in Resources)</returns>
        public static string CreateHierarchy(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            XElement result = new XElement("Root");

            AddGroupToXML(doc, result, "A");
            AddGroupToXML(doc, result, "B");

            return GetFlattenString(result);
        }

        private static void AddGroupToXML(XElement source, XElement result, string category)
        {
            IEnumerable<XElement> ElementsOfCategory = from el in source.Elements()
                                                       where (string)el.Element("Category") == category
                                                       select new XElement("Data", el.Element("Quantity"), el.Element("Price"));
            result.Add(new XElement("Group", new XAttribute("ID", category), ElementsOfCategory.ToList()));
        }

        /// <summary>
        /// Get list of orders numbers (where shipping state is NY) from xml representation
        /// </summary>
        /// <param name="xmlRepresentation">Orders xml representation (refer to PurchaseOrdersSourceFile.xml in Resources)</param>
        /// <returns>Concatenated orders numbers</returns>
        /// <example>
        /// 99301,99189,99110
        /// </example>
        public static string GetPurchaseOrders(string xmlRepresentation)
        {
            var result = from el in XElement.Parse(xmlRepresentation.Replace("aw:", string.Empty)).Elements()
                         where (string)el.Element("Address").Element("State") == "NY"
                         select el.Attribute("PurchaseOrderNumber").Value;
            return string.Join(",", result);
        }

        /// <summary>
        /// Reads csv representation and creates appropriate xml representation
        /// </summary>
        /// <param name="customers">Csv customers representation (refer to XmlFromCsvSourceFile.csv in Resources)</param>
        /// <returns>Xml customers representation (refer to XmlFromCsvResultFile.xml in Resources)</returns>
        public static string ReadCustomersFromCsv(string customers)
        {
            XElement doc = new XElement("Root");
            using (StringReader reader = new StringReader(customers))
            {
                string informationAboutCustomer;
                while ((informationAboutCustomer = reader.ReadLine()) != null)
                {
                    var queue = new Queue<string>(informationAboutCustomer.Split(','));
                    XNode source = new XElement("Customer", new XAttribute("CustomerID", queue.Dequeue()),
                        new XElement("CompanyName", queue.Dequeue()),
                        new XElement("ContactName", queue.Dequeue()),
                        new XElement("ContactTitle", queue.Dequeue()),
                        new XElement("Phone", queue.Dequeue()),
                        new XElement("FullAddress", new XElement("Address", queue.Dequeue()),
                            new XElement("City", queue.Dequeue()),
                            new XElement("Region", queue.Dequeue()),
                            new XElement("PostalCode", queue.Dequeue()),
                            new XElement("Country", queue.Dequeue())));
                    doc.Add(source);
                }
            }
            return GetFlattenString(doc);
        }

        /// <summary>
        /// Gets recursive concatenation of elements
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation of document with Sentence, Word and Punctuation elements. (refer to ConcatenationStringSource.xml in Resources)</param>
        /// <returns>Concatenation of all this element values.</returns>
        public static string GetConcatenationString(string xmlRepresentation)
        {
            XElement sentences = XElement.Parse(xmlRepresentation);

            var res = from el in sentences.Elements()
                      select el.Value;
            return string.Join("", res);
        }

        /// <summary>
        /// Replaces all "customer" elements with "contact" elements with the same childs
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with customers (refer to ReplaceCustomersWithContactsSource.xml in Resources)</param>
        /// <returns>Xml representation with contacts (refer to ReplaceCustomersWithContactsResult.xml in Resources)</returns>
        public static string ReplaceAllCustomersWithContacts(string xmlRepresentation)
        {
            XElement costomers = XElement.Parse(xmlRepresentation);
            foreach (var x in costomers.Elements()) x.Name = "contact";
            return GetFlattenString(costomers);
        }

        /// <summary>
        /// Finds all ids for channels with 2 or more subscribers and mark the "DELETE" comment
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with channels (refer to FindAllChannelsIdsSource.xml in Resources)</param>
        /// <returns>Sequence of channels ids</returns>
        public static IEnumerable<int> FindChannelsIds(string xmlRepresentation)
        {
            var result = from el in XElement.Parse(xmlRepresentation).Elements()
                         where el.Elements("subscriber").Count() >= 2 && el.LastNode.ToString() == "<!--DELETE-->"
                         select Convert.ToInt32(el.Attribute("id").Value);
            return result.ToArray();
        }

        /// <summary>
        /// Sort customers in docement by Country and City
        /// </summary>
        /// <param name="xmlRepresentation">Customers xml representation (refer to GeneralCustomersSourceFile.xml in Resources)</param>
        /// <returns>Sorted customers representation (refer to GeneralCustomersResultFile.xml in Resources)</returns>
        public static string SortCustomers(string xmlRepresentation)
        {
            var listSort = (from el in XElement.Parse(xmlRepresentation).Elements()
                            let country = (string)el.Element("FullAddress").Element("Country")
                            let city = (string)el.Element("FullAddress").Element("City")
                            orderby country, city
                            select el).ToList();
            XElement result = new XElement("Root");
            result.Add(listSort);
            return GetFlattenString(result);
        }

        /// <summary>
        /// Gets XElement flatten string representation to save memory
        /// </summary>
        /// <param name="xmlRepresentation">XElement object</param>
        /// <returns>Flatten string representation</returns>
        /// <example>
        ///     <root><element>something</element></root>
        /// </example>
        public static string GetFlattenString(XElement xmlRepresentation)
        {
            return xmlRepresentation.ToString();
        }

        /// <summary>
        /// Gets total value of orders by calculating products value
        /// </summary>
        /// <param name="xmlRepresentation">Orders and products xml representation (refer to GeneralOrdersFileSource.xml in Resources)</param>
        /// <returns>Total purchase value</returns>
        public static int GetOrdersValue(string xmlRepresentation)
        {
            var source = XElement.Parse(xmlRepresentation);

            return (from order in source.Element("Orders").Elements()
                    from product in source.Element("products").Elements()
                    let idProduct = (int)product.Attribute("Id")
                    let price = (int)product.Attribute("Value")
                    where (int)order.Element("product") == idProduct
                    select Tuple.Create(order.Element("EmployeeID").Value.Count(), price)).Sum(n => n.Item2);
        }
    }
}
