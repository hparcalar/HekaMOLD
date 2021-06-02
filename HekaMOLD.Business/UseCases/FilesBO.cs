using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Files;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class FilesBO : IBusinessObject
    {
        public AttachmentModel GetAttachment(int attachmentId)
        {
            AttachmentModel model = new AttachmentModel();

            try
            {
                var repo = _unitOfWork.GetRepository<Attachment>();
                var dbObj = repo.GetById(attachmentId);
                dbObj.MapTo(model);
            }
            catch (Exception)
            {

            }

            return model;
        }
        public AttachmentModel[] GetAttachmentList(int recordId, RecordType recordType)
        {
            AttachmentModel[] data = new AttachmentModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<Attachment>();

                List<AttachmentModel> result = new List<AttachmentModel>();

                repo.Filter(d => d.RecordId == recordId && d.RecordType == (int)recordType)
                    .ToList()
                    .ForEach(d => {
                        AttachmentModel containerObj = new AttachmentModel();
                        d.MapTo(containerObj);
                        containerObj.CreatedDateStr = string.Format("{0:dd.MM.yyyy HH:mm}",
                            d.CreatedDate);
                        result.Add(containerObj);
                    });

                data = result.ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult SaveAttachments(int recordId, RecordType recordType, AttachmentModel[] attachments)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Attachment>();

                var existingAttachments = repo.Filter(d => d.RecordId == recordId && d.RecordType == (int)recordType).ToArray();
                
                var willBeDeletedAttachments = existingAttachments.Where(d => !attachments.Any(m => m.Id == d.Id));
                foreach (var item in willBeDeletedAttachments)
                {
                    repo.Delete(item);
                }

                foreach (var item in attachments)
                {
                    var dbAttachment = repo.GetById(item.Id);
                    if (dbAttachment == null)
                    {
                        dbAttachment = new Attachment { RecordId = recordId, RecordType=(int)recordType,
                            CreatedDate = DateTime.Now };
                        repo.Add(dbAttachment);
                    }

                    item.RecordId = recordId;
                    item.RecordType = (int)recordType;

                    var bContent = dbAttachment.BinaryContent;
                    var crDate = dbAttachment.CreatedDate;
                    var crUser = dbAttachment.CreatedUserId;

                    item.MapTo(dbAttachment);

                    if (dbAttachment.BinaryContent == null)
                        dbAttachment.BinaryContent = bContent;
                    if (dbAttachment.CreatedDate == null)
                        dbAttachment.CreatedDate = crDate;
                    if (dbAttachment.CreatedUserId == null)
                        dbAttachment.CreatedUserId = crUser;
                }

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
    }
}
