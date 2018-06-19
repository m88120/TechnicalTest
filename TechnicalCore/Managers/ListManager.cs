using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCore.Context;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace TechnicalCore.Managers
{
    public class ListManager : IListManager
    {
        DbLeonContext _context;
        public ListManager(DbLeonContext context)
        {
            _context = context;
        }
        public ResponseModel<List<ExamStatusModel>> GetExamStatuses()
        {
            var response = new ResponseModel<List<ExamStatusModel>>();
            try
            {
                var statuses = _context.ExamStatuses.Where(x=>x.IsActive==true).Select(e => new ExamStatusModel { Id = e.Id, Name = e.Name }).ToList();
                response.Data = statuses;
                response.message = "success";
                response.status = true;
            }
            catch
            {
                response.message = "error";
                response.status = false;
            }
            return response;
        }

        public ResponseModel<List<SourceModel>> GetSources()
        {
            var response = new ResponseModel<List<SourceModel>>();
            try
            {
                var sources = _context.Sources.Select(s => new SourceModel { Id = s.Id, Name = s.Name }).ToList();
                response.Data = sources;
                response.message = "success";
                response.status = true;
            }
            catch
            {
                response.message = "error";
                response.status = false;
            }
            return response;
        }

        public ResponseModel<List<Exams>> GetExams()
        {
            var response = new ResponseModel<List<Exams>>();
            var exams = _context.Exams.Select(e => e).ToList();
            response.Data = exams;
            response.message = "success";
            response.status = true;
            return response;
        }
        public ResponseModel<List<ExamList>> GetExamList()
        {
            var response = new ResponseModel<List<ExamList>>();
            var exams = _context.Exams.Select(e => e).Where(e => e.IsActive == true).ToList();
            response.Data = exams.Select(x=>new ExamList { TestId=x.TestId,TestName=x.TestTitle }).ToList();
            response.message = "success";
            response.status = true;
            return response;
        }
    }
}
