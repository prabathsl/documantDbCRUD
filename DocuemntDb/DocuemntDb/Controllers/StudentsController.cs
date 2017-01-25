using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DocuemntDb.Models;
using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace DocuemntDb.Controllers
{
    public class StudentsController : ApiController
    {
        private DocuemntDbContext db;


        // GET: api/Students
        public async Task<IHttpActionResult> GetStudents()
        {
            try
            {
                db = new DocuemntDbContext();
                await db.Init();

                var result = db.ExecuteFromLinq();
                return Ok(result);
            }
            catch (DocumentClientException dce)
            {
                return BadRequest(dce.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Students/5
        [ResponseType(typeof(Student))]
        public async Task<IHttpActionResult> GetStudent(string id)
        {
            db = new DocuemntDbContext();
            await db.Init();

            Student student = db.ExecuteFromSQL(id);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/Students/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStudent(string id, Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != student.id)
            {
                return BadRequest();
            }



            try
            {
                db = new DocuemntDbContext();
                await db.Init();
                await db.UpdateDocument(id, student);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Students
        [ResponseType(typeof(Student))]
        public async Task<IHttpActionResult> PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                db = new DocuemntDbContext();
                await db.Init();

                await db.CreateDocumentIfNotExists(student);
            }
            catch (Exception ex)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = student.id }, student);
        }

        // DELETE: api/Students/5
        [ResponseType(typeof(Student))]
        public async Task<IHttpActionResult> DeleteStudent(string id)
        {
            try
            {
                db = new DocuemntDbContext();
                await db.Init();

                await db.Deleteocument(id);

                return Ok();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //private bool StudentExists(string id)
        //{
        //    return db.Students.Count(e => e.Id == id) > 0;
        //}
    }
}