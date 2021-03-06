﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PO_implementacja_StudiaPodyplomowe.Models;
using PO_implementacja_StudiaPodyplomowe.Models.Database;
using PO_implementacja_StudiaPodyplomowe.Models.Validators;
using System.Collections.Generic;
using System.Linq;

namespace PO_implementacja_StudiaPodyplomowe.Controllers.StudyFieldManager
{
    public class StudyFieldManagerFinalThesisApplicationsController : Controller
    {
        private IDao manager = DaoSingleton.GetInstance().Dao;

        public IActionResult Index()
        {
            List<SubmissionThesis> submissionTheses = manager.GetSubmissionTheses(1);
            ViewBag.isDataAvailable = submissionTheses.Count > 0;

            return View(submissionTheses);
        }

        public IActionResult Edit(int id)
        {
            SubmissionThesis submissionThesis = manager.GetSubmissionThesis(id);
            UpdateLecturersList(submissionThesis.FinalThesis.Lecturer.LecturerId);
            ViewBag.dataIsValid = true;

            return View(submissionThesis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection form)
        {
            List<bool> fieldsValidation = GetSubmissionFieldsValidation(form);
            bool dataIsValid = true;
            foreach (bool fieldValidation in fieldsValidation)
            {
                if (!fieldValidation)
                {
                    dataIsValid = false;
                }
            }
            ViewBag.dataIsValid = dataIsValid;
            ViewBag.form = form;

            if (!dataIsValid)
            {
                SubmissionThesis submission = manager.GetSubmissionThesis(id);
                UpdateLecturersList(int.Parse(form["LecturerId"]));
                ViewBag.fieldsValidation = fieldsValidation;
                return View(submission);
            }

            SubmissionThesis submissionThesis = new SubmissionThesis();
            submissionThesis.SubmissionId = id;
            submissionThesis.ThesisTopic = form["ThesisTopic"];
            submissionThesis.TopicNumber = int.Parse(form["TopicNumber"]);
            submissionThesis.ThesisObjectives = form["ThesisObjectives"];
            submissionThesis.ThesisScope = form["ThesisScope"];
            manager.EditSubmissionThesis(submissionThesis);
            int finalThesisId = manager.GetSubmissionThesis(id).FinalThesis.FinalThesisId;
            manager.EditFinalThesisLecturer(finalThesisId, int.Parse(form["LecturerId"]));
            return RedirectToAction("Index");
        }

        public IActionResult Confirm(int id)
        {
            manager.EditSubmissionThesesStatus(id, (int)ThesisStatus.APPROVED);
            return RedirectToAction("Index");
        }

        public IActionResult Discard(int id)
        {
            manager.EditSubmissionThesesStatus(id, (int)ThesisStatus.DISCARD);
            return RedirectToAction("Index");
        }
        public IActionResult Preview(int id)

        {
            SubmissionThesis submissionThesis = manager.GetSubmissionThesis(id);
            ViewData["StudyFieldManager"] = manager.GetStudyFieldManager(1);
            return View(submissionThesis);
        }


        private void UpdateLecturersList(int lecturerId)
        {
            List<Models.Lecturer> lecturers = manager.GetLecturers(1);
            IEnumerable<SelectListItem> selectList = from l in lecturers
                                                     select new SelectListItem
                                                     {
                                                         Value = l.LecturerId.ToString(),
                                                         Text = l.Name + " " + l.Surname
                                                     };
            ViewData["Lecturers"] = new SelectList(selectList, "Value", "Text", lecturerId);
        }

        private List<bool> GetSubmissionFieldsValidation(IFormCollection form)
        {
            List<bool> fieldsValidation = new List<bool>();
            fieldsValidation.Add(DataValidator.FieldContentIsValid(form["ThesisTopic"], maxLength: 2047));
            fieldsValidation.Add(DataValidator.NumberIsValid(form["TopicNumber"], maxRange: 99999));
            fieldsValidation.Add(DataValidator.FieldContentIsValid(form["ThesisObjectives"], maxLength: 2047));
            fieldsValidation.Add(DataValidator.FieldContentIsValid(form["ThesisScope"], maxLength: 2047));

            return fieldsValidation;
        }
    }
}
