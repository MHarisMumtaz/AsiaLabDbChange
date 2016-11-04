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

        public List<ReferDoctorCommision> GetByReferDoctorId(int ReferId)
        {
            var query = (from refr in _ReferDoctorCommisionRepository.Table
                         where refr.ReferDoctorId == ReferId
                         select refr).ToList<ReferDoctorCommision>();
            return query;
            
        }
        public void Delete(ReferDoctorCommision DoctorCommision)
        {
            _ReferDoctorCommisionRepository.Delete(DoctorCommision);
        }

        public void Delete(List<ReferDoctorCommision> DoctorCommisions)
        {
            foreach (var item in DoctorCommisions)
            {
                _ReferDoctorCommisionRepository.Delete(item);        
            }
        
        }
    }
}