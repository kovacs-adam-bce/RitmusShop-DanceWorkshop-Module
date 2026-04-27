/*
' Copyright (c) 2026 SyntaxSalsa
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

//using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search;
using System.Collections.Generic;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Components
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Controller class for Dnn.SyntaxSalsa.DanceWorkShop
    /// 
    /// The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    /// DotNetNuke will poll this class to find out which Interfaces the class implements. 
    /// 
    /// The IPortable interface is used to import/export content from a DNN module
    /// 
    /// The ISearchable interface is used by DNN to index the content of a module
    /// 
    /// The IUpgradeable interface allows module developers to execute code during the upgrade 
    /// process for a module.
    /// 
    /// Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------

    //uncomment the interfaces to add the support.
    public class FeatureController //: IPortable, ISearchable, IUpgradeable
    {


        #region Optional Interfaces

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be exported</param>
        /// -----------------------------------------------------------------------------
        //public string ExportModule(int ModuleID)
        //{
        //string strXML = "";

        //List<Dnn.SyntaxSalsa.DanceWorkShopInfo> colDnn.SyntaxSalsa.DanceWorkShops = GetDnn.SyntaxSalsa.DanceWorkShops(ModuleID);
        //if (colDnn.SyntaxSalsa.DanceWorkShops.Count != 0)
        //{
        //    strXML += "<Dnn.SyntaxSalsa.DanceWorkShops>";

        //    foreach (Dnn.SyntaxSalsa.DanceWorkShopInfo objDnn.SyntaxSalsa.DanceWorkShop in colDnn.SyntaxSalsa.DanceWorkShops)
        //    {
        //        strXML += "<Dnn.SyntaxSalsa.DanceWorkShop>";
        //        strXML += "<content>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDnn.SyntaxSalsa.DanceWorkShop.Content) + "</content>";
        //        strXML += "</Dnn.SyntaxSalsa.DanceWorkShop>";
        //    }
        //    strXML += "</Dnn.SyntaxSalsa.DanceWorkShops>";
        //}

        //return strXML;

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be imported</param>
        /// <param name="Content">The content to be imported</param>
        /// <param name="Version">The version of the module to be imported</param>
        /// <param name="UserId">The Id of the user performing the import</param>
        /// -----------------------------------------------------------------------------
        //public void ImportModule(int ModuleID, string Content, string Version, int UserID)
        //{
        //XmlNode xmlDnn.SyntaxSalsa.DanceWorkShops = DotNetNuke.Common.Globals.GetContent(Content, "Dnn.SyntaxSalsa.DanceWorkShops");
        //foreach (XmlNode xmlDnn.SyntaxSalsa.DanceWorkShop in xmlDnn.SyntaxSalsa.DanceWorkShops.SelectNodes("Dnn.SyntaxSalsa.DanceWorkShop"))
        //{
        //    Dnn.SyntaxSalsa.DanceWorkShopInfo objDnn.SyntaxSalsa.DanceWorkShop = new Dnn.SyntaxSalsa.DanceWorkShopInfo();
        //    objDnn.SyntaxSalsa.DanceWorkShop.ModuleId = ModuleID;
        //    objDnn.SyntaxSalsa.DanceWorkShop.Content = xmlDnn.SyntaxSalsa.DanceWorkShop.SelectSingleNode("content").InnerText;
        //    objDnn.SyntaxSalsa.DanceWorkShop.CreatedByUser = UserID;
        //    AddDnn.SyntaxSalsa.DanceWorkShop(objDnn.SyntaxSalsa.DanceWorkShop);
        //}

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchItems implements the ISearchable Interface
        /// </summary>
        /// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
        /// -----------------------------------------------------------------------------
        //public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(DotNetNuke.Entities.Modules.ModuleInfo ModInfo)
        //{
        //SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();

        //List<Dnn.SyntaxSalsa.DanceWorkShopInfo> colDnn.SyntaxSalsa.DanceWorkShops = GetDnn.SyntaxSalsa.DanceWorkShops(ModInfo.ModuleID);

        //foreach (Dnn.SyntaxSalsa.DanceWorkShopInfo objDnn.SyntaxSalsa.DanceWorkShop in colDnn.SyntaxSalsa.DanceWorkShops)
        //{
        //    SearchItemInfo SearchItem = new SearchItemInfo(ModInfo.ModuleTitle, objDnn.SyntaxSalsa.DanceWorkShop.Content, objDnn.SyntaxSalsa.DanceWorkShop.CreatedByUser, objDnn.SyntaxSalsa.DanceWorkShop.CreatedDate, ModInfo.ModuleID, objDnn.SyntaxSalsa.DanceWorkShop.ItemId.ToString(), objDnn.SyntaxSalsa.DanceWorkShop.Content, "ItemId=" + objDnn.SyntaxSalsa.DanceWorkShop.ItemId.ToString());
        //    SearchItemCollection.Add(SearchItem);
        //}

        //return SearchItemCollection;

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeModule implements the IUpgradeable Interface
        /// </summary>
        /// <param name="Version">The current version of the module</param>
        /// -----------------------------------------------------------------------------
        //public string UpgradeModule(string Version)
        //{
        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        #endregion

    }

}
