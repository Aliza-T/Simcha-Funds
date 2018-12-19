using SimchaFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.Models
{
    public class ContributorViewModel
    {
        public IEnumerable<ContributorsBalance> contributors { get; set; }
        public int Balance { get; set; }

    }
    public class ContributorsBalance
    {
        public int Balance { get; set; }
        public Contributors contributors { get; set; }
    }
    public class IndexViewModel
    {
        public IEnumerable<SimchaViewModel> svm { get; set; }
        public int Count { get; set; }
    }
    public class SimchaViewModel
    {
        public Simcha Simcha { get; set; }
        public int ContributorCount { get; set; }
    }
    public class ContributionsViewModel
    {
        public IEnumerable<SimchaDonation> simchaDonation { get; set; }
        public Simcha Simcha { get; set; }



    }
    public class EmailContributorVM
    {
        public IEnumerable<Contributors> Contributors { get; set; }
        public string SimchaName { get; set; }
    }
    public class HistoryViewModel
    {
        public List<DetailsViewModel> ContributorHistory { get; set; }
        public string Name { get; set; }

    }
    public class DetailsViewModel
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }


    }

}