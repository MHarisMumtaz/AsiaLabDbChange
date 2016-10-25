using AsiaLabv1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsiaLabv1.Services
{
    public class ReferDoctorCommisionService
    {
        Repository<ReferDoctorCommision> _ReferDoctorCommisionRepository = new Repository<ReferDoctorCommision>();

        public void Add(ReferDoctorCommision DoctorCommision)
        {
            _ReferDoctorCommisionRepository.Insert(DoctorCommision);
        }
    }
}