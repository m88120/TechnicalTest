using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechnicalCore.Context;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace TechnicalCore.Managers
{
   public class ExamStatusManager:IExamStatusManager
    {
        DbLeonContext _context;

        public ExamStatusManager(DbLeonContext dbcontext)
        {
            this._context = dbcontext;
        }

        public ResponseModel<List<ExamStatusModel>> ExamStatusList()
        {
            ResponseModel<List<ExamStatusModel>> result = new ResponseModel<List<ExamStatusModel>> { Data = new List<ExamStatusModel>() };
            try
            {
                var lst = _context.ExamStatuses.Where(x=>x.IsActive==true);
                foreach (var item in lst)
                {
                    ExamStatusModel model = new ExamStatusModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    model.IsActive = item.IsActive;
                    result.Data.Add(model);
                }
                result.status = true;
                result.message = "success";
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
            }
            return result;
        }
        public ResponseModel<ExamStatusModel> Delete(int id)
        {
            ResponseModel<ExamStatusModel> result = new ResponseModel<ExamStatusModel> { Data = new ExamStatusModel() };
            try
            {
                var item = _context.ExamStatuses.Include("ExamDetails").Where(e => e.Id == id).Select(e => e).FirstOrDefault();
                if (item != null)
                {                  
                    //_context.ExamStatuses.Remove(item);
                    item.IsActive = false;
                    _context.SaveChanges();
                    result.status = true;
                    result.message = "Success";
                }
                else
                {
                    result.status = false;
                    result.message = "Status Not Found";
                }
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
            }
            return result;
        }
        public ResponseModel<ExamStatusModel> Edit(int id)
        {
            ResponseModel<ExamStatusModel> result = new ResponseModel<ExamStatusModel> { Data = new ExamStatusModel() };
            try
            {
                var item = _context.ExamStatuses.Where(e => e.Id == id).Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    result.status = true;
                    result.Data.Id = item.Id;
                    result.Data.Name = item.Name;                   
                }
                else
                {
                    result.status = false;
                    result.message = "Test Not Found";
                }
            }
            catch (Exception ex)
            {
                result.message = ex.Message;

            }
            return result;
        }
        public ResponseModel<ExamStatusModel> Save(ExamStatusModel model)
        {
            ResponseModel<ExamStatusModel> result = new ResponseModel<ExamStatusModel> { Data = new ExamStatusModel() };
            try
            {
                ExamStatuses db = new ExamStatuses();
                db.Name = model.Name;
                db.IsActive = true;
                _context.ExamStatuses.Add(db);
                _context.SaveChanges();
                result = new ResponseModel<ExamStatusModel> { status = true, message = "Success", Data = new ExamStatusModel() { Id = db.Id, Name = db.Name } };
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = ex.Message;
                result.Data = new ExamStatusModel() { Id = 0, Name = null };
            }
            return result;
        }

        public ResponseModel<ExamStatusModel> Update(ExamStatusModel model)
        {
            ResponseModel<ExamStatusModel> result = new ResponseModel<ExamStatusModel> { Data = new ExamStatusModel() };
            try
            {
                var item = _context.ExamStatuses.Where(e => e.Id == model.Id).Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    item.Name = model.Name;
                    //item.IsActive = model.IsActive;
                    _context.SaveChanges();
                    result = new ResponseModel<ExamStatusModel> { status = true, message = "Success" };
                }
                else
                {
                    result = new ResponseModel<ExamStatusModel> { status = false, message = "Status Not Found" };
                }
            }
            catch (Exception ex)
            {
                result = new ResponseModel<ExamStatusModel> { status = false, message = ex.Message.ToString() };
            }
            return result;
        }
    }
}
