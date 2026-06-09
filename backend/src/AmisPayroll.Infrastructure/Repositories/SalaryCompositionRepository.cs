using System;
using System.Threading.Tasks;
using AmisPayroll.Application.Interfaces.Repositories;
using AmisPayroll.Entities.Entities;
using AmisPayroll.Infrastructure.Context;
using Dapper;

namespace AmisPayroll.Infrastructure.Repositories
{
    public class SalaryCompositionRepository : BaseRepository<SalaryComposition>, ISalaryCompositionRepository
    {
        public SalaryCompositionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _tableName = "pa_salary_composition";
            _primaryKeyColumn = "composition_id";
        }

        public async Task<bool> CheckDuplicateCodeAsync(string code, Guid? currentId = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE composition_code = @Code";
            
            if (currentId.HasValue)
            {
                sql += $" AND {_primaryKeyColumn} != @Id";
                return await connection.ExecuteScalarAsync<bool>(sql, new { Code = code, Id = currentId });
            }
            
            return await connection.ExecuteScalarAsync<bool>(sql, new { Code = code });
        }

        public override async Task<int> InsertAsync(SalaryComposition entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = $@"INSERT INTO {_tableName} (composition_id, organization_id, composition_code, composition_name, composition_type, composition_nature, tax_nature, norm_formula, is_allow_exceed_norm, value_type, amount, calculation_formula, description, is_display_on_payslip, source_type, status) 
                         VALUES (@CompositionId, @OrganizationId, @CompositionCode, @CompositionName, @CompositionType, @CompositionNature, @TaxNature, @NormFormula, @IsAllowExceedNorm, @ValueType, @Amount, @CalculationFormula, @Description, @IsDisplayOnPayslip, @SourceType, @Status)";
            return await connection.ExecuteAsync(sql, entity);
        }

        public override async Task<int> UpdateAsync(SalaryComposition entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = $@"UPDATE {_tableName} SET organization_id = @OrganizationId, composition_code = @CompositionCode, composition_name = @CompositionName, composition_type = @CompositionType, composition_nature = @CompositionNature, tax_nature = @TaxNature, norm_formula = @NormFormula, is_allow_exceed_norm = @IsAllowExceedNorm, value_type = @ValueType, amount = @Amount, calculation_formula = @CalculationFormula, description = @Description, is_display_on_payslip = @IsDisplayOnPayslip, source_type = @SourceType, status = @Status 
                         WHERE {_primaryKeyColumn} = @CompositionId";
            return await connection.ExecuteAsync(sql, entity);
        }
    }
}