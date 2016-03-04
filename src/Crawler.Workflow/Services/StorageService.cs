﻿using System;
using System.Collections.Generic;
using System.Linq;
using Crawler.Workflow.ExternMicroservices;
using Crawler.Workflow.Models;

namespace Crawler.Workflow.Services
{
    public interface IStorageService
    {
        IEnumerable<Site> GetSites();
        IEnumerable<Page> GetPagesByState(int id, int state);
        void AddPage(Page page);
        void UpdatePage(Page page);
        string AddPages(IEnumerable<Page> pages);
    }

    public class StorageService: IStorageService
    {
        private readonly IPageMicroservice pageMicroservice;

        public StorageService(
            IPageMicroservice pageMicroservice)
        {
            if (pageMicroservice == null)
            {
                throw new ArgumentNullException(nameof(pageMicroservice));
            }
            this.pageMicroservice = pageMicroservice;
        }

        public IEnumerable<Site> GetSites() => pageMicroservice.GetSites();

        public IEnumerable<Page> GetPagesByState(int id, int state) => pageMicroservice.GetPagesbyState(id, state);

        public void AddPage(Page page) => pageMicroservice.PostPage(page);

        public void UpdatePage(Page page) => pageMicroservice.PutPage(page);

        public string AddPages(IEnumerable<Page> pages)
        {
            int saved = 0;

            const int size = 500;

            int count = pages.Count();

            int countPaginate = Convert.ToInt32(count / size) + 1;

            for (int i = 0; i < countPaginate; i++)
            {
                var selectPages = pages.Skip(i * size).Take(size).ToList();

                saved += pageMicroservice.PostPages(selectPages);
            }

            return $"{saved} / {count}";
        }
    }
}