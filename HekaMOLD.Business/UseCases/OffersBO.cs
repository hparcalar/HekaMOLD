using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Offers;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using HekaMOLD.Business.UseCases.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class OffersBO : CoreReceiptsBO
    {
        public ItemOfferModel[] GetItemOfferList()
        {
            List<ItemOfferModel> data = new List<ItemOfferModel>();

            var repo = _unitOfWork.GetRepository<ItemOffer>();

            return repo.GetAll().ToList()
                .Select(d => new ItemOfferModel
                {
                    Id = d.Id,
                    OfferDate = d.OfferDate,
                    OfferDateStr = string.Format("{0:dd.MM.yyyy}", d.OfferDate),
                    FirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                    FirmName = d.Firm != null ? d.Firm.FirmName : "",
                    OfferNo = d.OfferNo,
                    Explanation = d.Explanation,
                    FirmId = d.FirmId,
                    PlantId = d.PlantId,
                })
                .OrderByDescending(d => d.OfferNo)
                .ToArray();
        }

        public BusinessResult SaveOrUpdateItemOffer(ItemOfferModel model, bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOffer>();
                var repoDetail = _unitOfWork.GetRepository<ItemOfferDetail>();
                var repoDetailRoute = _unitOfWork.GetRepository<ItemOfferDetailRoutePricing>();

                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemOffer();
                    dbObj.OfferNo = GetNextOfferNo();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                    newRecord = true;
                }

                if (!string.IsNullOrEmpty(model.OfferDateStr))
                {
                    model.OfferDate = DateTime.ParseExact(model.OfferDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }

                var crDate = dbObj.CreatedDate;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                if (model.Details == null && detailCanBeNull == false)
                    throw new Exception("Detay bilgisi olmadan teklif kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                // DELETED DETAILS
                var newDetailIdList = model.Details.Select(d => d.Id).ToArray();
                var deletedDetails = dbObj.ItemOfferDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                foreach (var item in deletedDetails)
                {
                    if (item.ItemOfferDetailRoutePricing.Any())
                    {
                        var pricingList = item.ItemOfferDetailRoutePricing.ToArray();
                        foreach (var pricingItem in pricingList)
                        {
                            repoDetailRoute.Delete(pricingItem);
                        }
                    }

                    repoDetail.Delete(item);
                }

                int lineNo = 1;
                foreach (var item in model.Details)
                {
                    var dbDetail = repoDetail.Get(d => d.Id == item.Id);
                    if (dbDetail == null)
                    {
                        dbDetail = new ItemOfferDetail
                        {
                            ItemOffer = dbObj,
                        };

                        repoDetail.Add(dbDetail);
                    }

                    item.MapTo(dbDetail);
                    dbDetail.ItemOffer = dbObj;

                    #region FIND OR CREATE ITEM FROM ITEM-EXPLANATION
                    if (dbDetail.ItemId == null)
                    {
                        using (DefinitionsBO defBO = new DefinitionsBO())
                        {
                            var explanationForItemName = dbDetail.ItemExplanation
                                .Replace(".dft", "")
                                .Replace(".DFT", "");

                            var foundItem = defBO.FindItem(explanationForItemName);
                            if (foundItem != null && foundItem.Id > 0)
                                dbDetail.ItemId = foundItem.Id;
                            else
                            {
                                var piecesTypeUnit = defBO.GetUnitType("ADET");
                                if (piecesTypeUnit == null || piecesTypeUnit.Id <= 0)
                                    piecesTypeUnit = defBO.GetUnitType("ADT");

                                var crItemResult = defBO.SaveOrUpdateItem(new Models.DataTransfer.Core.ItemModel
                                {
                                    ItemName = explanationForItemName,
                                    PlantId = dbObj.PlantId,
                                    ItemType = (int)ItemType.Product,
                                    Units = new Models.DataTransfer.Core.ItemUnitModel[] { 
                                        new Models.DataTransfer.Core.ItemUnitModel
                                        {
                                            IsMainUnit = true,
                                            DividerFactor = 1,
                                            MultiplierFactor = 1,
                                            UnitId = piecesTypeUnit.Id,
                                            NewDetail = true,
                                        }
                                    },
                                    ItemNo = defBO.GenerateProductNoFromName(explanationForItemName),
                                    CreatedDate = DateTime.Now,
                                });

                                if (crItemResult.Result)
                                {
                                    dbDetail.ItemId = crItemResult.RecordId;
                                }
                            }
                        }
                    }
                    #endregion

                    #region SAVE ROUTE PRICINGS
                    var exPricings = dbDetail.ItemOfferDetailRoutePricing.ToArray();
                    foreach (var exPrc in exPricings)
                    {
                        repoDetailRoute.Delete(exPrc);
                    }

                    if (item.ProcessList != null)
                    {
                        foreach (var nwPrc in item.ProcessList)
                        {
                            var dbNewPricing = new ItemOfferDetailRoutePricing
                            {
                                ItemOfferDetail = dbDetail,
                                RouteItemId = nwPrc.RouteItemId,
                                UnitPrice = nwPrc.UnitPrice,
                                TotalPrice = nwPrc.TotalPrice,
                            };
                            repoDetailRoute.Add(dbNewPricing);
                        }
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(item.ItemVisualStr))
                    {
                        dbDetail.ItemVisual = Convert.FromBase64String(item.ItemVisualStr);
                    }

                    if (dbObj.Id > 0)
                        dbDetail.ItemOfferId = dbObj.Id;

                    lineNo++;
                }
                #endregion

                _unitOfWork.SaveChanges();

                result.Result = true;
                result.RecordId = dbObj.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ItemOfferDetailRoutePricingModel[] GetPricingsByRoute(int routeId)
        {
            ItemOfferDetailRoutePricingModel[] data = new ItemOfferDetailRoutePricingModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<RouteItem>();
                data = repo.Filter(d => d.RouteId == routeId)
                    .OrderBy(d => d.LineNumber)
                    .Select(d => new ItemOfferDetailRoutePricingModel
                    {
                        Id = 0,
                        RouteItemId = d.Id,
                        ProcessCode = d.Process.ProcessCode,
                        ProcessName = d.Process.ProcessName,
                        RouteCode = d.Route.RouteCode,
                        RouteName = d.Route.RouteName,
                        UnitPrice = d.Process.UnitPrice,
                        TotalPrice = 0,
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult DeleteItemOffer(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOffer>();
                var repoDetail = _unitOfWork.GetRepository<ItemOfferDetail>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Silinmesi istenen teklif kaydına ulaşılamadı.");

                // CLEAR DETAILS
                if (dbObj.ItemOfferDetail.Any())
                {
                    var detailObjArr = dbObj.ItemOfferDetail.ToArray();
                    foreach (var item in detailObjArr)
                    {
                        repoDetail.Delete(item);
                    }
                }

                repo.Delete(dbObj);
                _unitOfWork.SaveChanges();

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ItemOfferModel GetItemOffer(int id)
        {
            ItemOfferModel model = new ItemOfferModel { Details = new ItemOfferDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemOffer>();
            var repoDetails = _unitOfWork.GetRepository<ItemOfferDetail>();

            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.OfferDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.OfferDate);
                model.FirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                model.FirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                model.HasAnyOrder = dbObj.ItemOfferDetail.Any(m => m.ItemOrderDetail.Any());
                model.CreatedUserName = dbObj.CreatedUser != null ? dbObj.CreatedUser.UserName : "";
                model.FirmResponsible = dbObj.Firm != null ?
                    (
                        dbObj.Firm.FirmAuthor.Any() ? dbObj.Firm.FirmAuthor.Select(d => d.AuthorName).FirstOrDefault()
                            : dbObj.Firm.FirmName
                    ) 
                    : "";

                model.Details =
                    repoDetails.Filter(d => d.ItemOfferId == dbObj.Id)
                    .ToList()
                    .Select(d => new ItemOfferDetailModel
                    {
                        Id = d.Id,
                        ItemId = d.ItemId,
                        ItemNo = d.Item != null ? d.Item.ItemNo : "",
                        ItemName = d.Item != null ? d.Item.ItemName : "",
                        CreditMonths = d.CreditMonths,
                        CreditRate = d.CreditRate,
                        ItemExplanation = d.ItemExplanation,
                        ItemOfferId = d.ItemOfferId,
                        ItemVisual = d.ItemVisual,
                        LaborCost = d.LaborCost,
                        NewDetail = false,
                        ProfitRate = d.ProfitRate,
                        RoutePrice = d.RoutePrice,
                        SheetTickness = d.SheetTickness,
                        QualityExplanation = d.QualityExplanation,
                        RouteId = d.RouteId,
                        Quantity = d.Quantity,
                        SheetWeight = d.SheetWeight,
                        TotalPrice = d.TotalPrice,
                        UnitPrice = d.UnitPrice,
                        WastageWeight = d.WastageWeight,
                        RouteCode = d.Route != null ? d.Route.RouteCode : "",
                        RouteName = d.Route != null ? d.Route.RouteName : "",
                        ItemVisualStr = d.ItemVisual != null ?
                            (Convert.ToBase64String(d.ItemVisual)) : "",
                        ProcessList = d.ItemOfferDetailRoutePricing
                            .Select(m => new ItemOfferDetailRoutePricingModel
                            {
                                Id = m.Id,
                                RouteItemId = m.RouteItemId,
                                ProcessCode = m.RouteItem.Process.ProcessCode,
                                ProcessName = m.RouteItem.Process.ProcessName,
                                RouteCode = m.RouteItem.Route.RouteCode,
                                RouteName = m.RouteItem.Route.RouteName,
                                UnitPrice = m.UnitPrice,
                                TotalPrice = m.TotalPrice,
                            }).ToArray()
                    }).ToArray();

                if (model.HasAnyOrder)
                {
                    var orderedOfferDetail = dbObj.ItemOfferDetail.FirstOrDefault(d => d.ItemOrderDetail.Any());
                    var dbOrderDetail = orderedOfferDetail.ItemOrderDetail.FirstOrDefault();
                    model.ItemOrderId = dbOrderDetail.ItemOrderId.Value;
                    model.ItemOrderNo = dbOrderDetail.ItemOrder.OrderNo;
                }
            }

            return model;
        }

        public BusinessResult CreateSaleOrder(int offerId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOffer>();
                var repoOfferDetail = _unitOfWork.GetRepository<ItemOfferDetail>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();

                var dbOffer = repo.Get(d => d.Id == offerId);
                if (dbOffer == null)
                    throw new Exception("Teklif bilgisi bulunamadı.");

                List<ItemOrderDetailModel> newOrderDetails = new List<ItemOrderDetailModel>();

                int lineNumber = 1;
                var offerDetails = dbOffer.ItemOfferDetail.ToArray();

                using (OrdersBO bObj = new OrdersBO())
                {
                    foreach (var item in offerDetails)
                    {
                        if (item.ItemId == null)
                            throw new Exception("Teklifi siparişe dönüştürebilmek için sistem üzerinde kayıtlı olan ürünlerden seçim yapmalısınız.");

                        if (repoOrderDetail.Any(d => d.ItemOfferDetailId == item.Id))
                            throw new Exception("Daha önce siparişe dönüştürülen bir teklif üzerinden yeniden transfer yapılamaz.");

                        var newDetail = new ItemOrderDetailModel
                        {
                            ItemOfferDetailId = item.Id,
                            NewDetail = true,
                            ItemId = item.ItemId,
                            LineNumber = lineNumber,
                            CreatedDate = DateTime.Now,
                            UnitPrice = item.UnitPrice + (item.RoutePrice ?? 0),
                            TaxIncluded = false,
                            TaxRate = 18,
                            SubTotal = item.TotalPrice,
                            Quantity = item.Quantity,
                        };

                        newDetail = bObj.CalculateOrderDetail(newDetail);

                        newOrderDetails.Add(newDetail);

                        lineNumber++;
                    }
                }

                using (OrdersBO bObj = new OrdersBO())
                {
                    result = bObj.SaveOrUpdateItemOrder(new Models.DataTransfer.Order.ItemOrderModel { 
                        PlantId = 1,
                        OrderDate = DateTime.Now,
                        OrderNo = bObj.GetNextOrderNo(1, Models.Constants.ItemOrderType.Sale),
                        CreatedDate = DateTime.Now,
                        OrderType = (int)ItemOrderType.Sale,
                        OrderStatus = 1,
                        Explanation = dbOffer.Explanation,
                        FirmId = dbOffer.FirmId,
                        Details = newOrderDetails.ToArray(),
                    }, false);
                }

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ItemOfferDetailModel CalculateOfferDetail(ItemOfferDetailModel model)
        {
            decimal? subTotal = 0;

            subTotal = (model.UnitPrice + (model.RoutePrice ?? 0)) * model.Quantity;

            model.TotalPrice = subTotal;

            return model;
        }
    }
}
