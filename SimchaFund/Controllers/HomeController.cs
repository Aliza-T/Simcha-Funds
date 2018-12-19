using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaFund.Data;
using SimchaFund.Models;

namespace SimchaFund.Controllers
{
    public class HomeController : Controller
    {

        SimchaFundsDB DB = new SimchaFundsDB(Properties.Settings.Default.ConStr);
        public ActionResult Index()
        {
            var simchas = DB.GetAllSimchas();
            var simchaViewModel = new List<SimchaViewModel>();
            foreach (Simcha s in simchas)
            {
                SimchaViewModel svm = new SimchaViewModel();
                svm.Simcha = s;
                svm.Simcha.Total = DB.Total(s.Id);
                svm.ContributorCount = DB.GetContibutorsCountForSimcha(s.Id);
                simchaViewModel.Add(svm);
            }
            IndexViewModel VM = new IndexViewModel
            {
                Count = DB.GetContibutorsCount(),
                svm = simchaViewModel
            };
            return View(VM);
        }
        public ActionResult Contributors()
        {
            ContributorViewModel VM = new ContributorViewModel();
            var contributors = DB.GetContibutors();
            var CB = new List<ContributorsBalance>();
            foreach (var c in contributors)
            {
                var cb = new ContributorsBalance();
                cb.Balance = DB.GetBalance(c.Id);
                cb.contributors = c;
                CB.Add(cb);
            }
            VM.contributors = CB;
            VM.Balance = DB.GetTotalBalance();
            return View(VM);
        }

        public ActionResult Contributions(int simchaId)
        {
            ContributionsViewModel VM = new ContributionsViewModel();
            VM.Simcha = DB.GetSimcha(simchaId);
            VM.simchaDonation = DB.GetContibutorsforDonations(simchaId);
            return View(VM);
        }
        public ActionResult History(int id)
        {
            HistoryViewModel VM = new HistoryViewModel();
            VM.Name = DB.GetContributorName(id);
            var deposits = DB.GetDepositForContributor(id);
            var donations = DB.GetDonationForContributor(id);
            List<DetailsViewModel> detailsView = new List<DetailsViewModel>();
            IEnumerable<DetailsViewModel> dvm = deposits.Select(d => new DetailsViewModel
            {
                Action = "Deposit",
                Date = d.Date,
                Amount = d.DepositAmount

            });
            IEnumerable<DetailsViewModel> vm = donations.Select(d => new DetailsViewModel
            {
                Action = "Donation",
                Date = d.Date,
                Amount = -d.DonationAmount
            });
            foreach (var item in dvm)
            {
                detailsView.Add(item);
            }
            foreach (var item in vm)
            {
                detailsView.Add(item);
            }
            detailsView.OrderByDescending(d => d.Date);
            VM.ContributorHistory = detailsView;
            return View(VM);
        }
        [HttpPost]
        public ActionResult AddDeposit(Deposit deposit)
        {
            DB.AddDeposit(deposit);
            return Redirect("/home/Contributors");
        }
        [HttpPost]
        public ActionResult AddContributor(Contributors c)
        {
            DB.AddContributor(c);
            return Redirect("/home/Contributors");
        }
        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            DB.AddSimcha(simcha);
            return Redirect("/home/Index");
        }
        [HttpPost]
        public ActionResult Update(Contributors contributors)
        {
            DB.Update(contributors);
            return Redirect("/home/Contributors");
        }

        [HttpPost]
        public ActionResult UpdateDonations(List<SimchaDonation> contributions, int simchaId)
        {

            DB.UpdateDonations(contributions, simchaId);
            return Redirect("/home/Index");
        }

        public ActionResult EmailContributors(int id)
        {
            EmailContributorVM VM = new EmailContributorVM();
            VM.Contributors = DB.GetContributorsForSimcha(id);
            VM.SimchaName = DB.GetSimcha(id).Name;
            return View(VM);
        }



    }
}