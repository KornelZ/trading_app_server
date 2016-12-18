using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using LGSA_Server.Model.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LGSA.Model.Services
{
    public class AuthenticationService : IDataService<users_Authetication>
    {
        private IUnitOfWorkFactory _factory;
        public AuthenticationService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public async Task<ErrorValue> Add(users_Authetication entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    var result = unitOfWork.AuthenticationRepository.Add(entity);
                    if(result == null)
                    {
                        return ErrorValue.EntityExists;
                    }
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch(Exception)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }

        public async Task<ErrorValue> Delete(users_Authetication entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.AuthenticationRepository.Delete(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }

        public async Task<users_Authetication> GetById(int id)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entity = await unitOfWork.AuthenticationRepository.GetById(id);
                    return entity;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public async Task<IEnumerable<users_Authetication>> GetData(Expression<Func<users_Authetication, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.AuthenticationRepository.GetData(filter);

                    return entities;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public async Task<ErrorValue> Update(users_Authetication entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.AuthenticationRepository.Update(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }
    }
}
