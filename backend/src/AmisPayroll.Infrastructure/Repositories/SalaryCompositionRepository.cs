using AmisPayroll.Application.Interfaces.Repositories;
using AmisPayroll.Entities.Entities;
using AmisPayroll.Infrastructure.Context;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AmisPayroll.Infrastructure.Repositories
{
    public class SalaryCompositionRepository : BaseRepository<SalaryComposition>, ISalaryCompositionRepository
    {
        public SalaryCompositionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _tableName = "pa_salary_composition";
            _primaryKeyColumn = "composition_id";
        }

        public async Task<bool> CheckDuplicateCodeAsync(string code)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "SELECT COUNT(1) FROM pa_salary_composition WHERE composition_code = @Code";
            int count = await connection.ExecuteScalarAsync<int>(sql, new { Code = code });
            return count > 0;
        }

        public async Task<bool> CheckDuplicateCodeAsync(string code, Guid excludeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT COUNT(1)
                FROM pa_salary_composition
                WHERE composition_code = @Code AND composition_id <> @ExcludeId";
            int count = await connection.ExecuteScalarAsync<int>(sql, new { Code = code, ExcludeId = excludeId });
            return count > 0;
        }

        public async Task<(IEnumerable<dynamic> Data, int TotalCount)> GetPagingAsync(int skip, int take, string? searchValue)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            string searchPattern = $"%{searchValue ?? ""}%";

            string sql = @"
                SELECT COUNT(1) 
                FROM pa_salary_composition 
                WHERE composition_code LIKE @Search OR composition_name LIKE @Search;

                SELECT sc.*, org.organization_name AS OrganizationName
                FROM pa_salary_composition sc
                LEFT JOIN pa_organization org ON sc.organization_id = org.organization_id
                WHERE sc.composition_code LIKE @Search OR sc.composition_name LIKE @Search
                LIMIT @Take OFFSET @Skip;";

            var parameters = new { Search = searchPattern, Take = take, Skip = skip };

            using var multi = await connection.QueryMultipleAsync(sql, parameters);
            int totalCount = await multi.ReadFirstAsync<int>();
            var data = await multi.ReadAsync<dynamic>();

            return (data, totalCount);
        }

        public override async Task<int> InsertAsync(SalaryComposition entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO pa_salary_composition (
                    composition_id,
                    organization_id,
                    composition_code,
                    composition_name,
                    composition_type,
                    composition_nature,
                    tax_nature,
                    norm_formula,
                    is_allow_exceed_norm,
                    value_type,
                    amount,
                    calculation_formula,
                    description,
                    is_display_on_payslip,
                    source_type,
                    status
                ) VALUES (
                    @CompositionId,
                    @OrganizationId,
                    @CompositionCode,
                    @CompositionName,
                    @CompositionType,
                    @CompositionNature,
                    @TaxNature,
                    @NormFormula,
                    @IsAllowExceedNorm,
                    @ValueType,
                    @Amount,
                    @CalculationFormula,
                    @Description,
                    @IsDisplayOnPayslip,
                    @SourceType,
                    @Status
                );";

            return await connection.ExecuteAsync(sql, entity);
        }

        public override async Task<int> UpdateAsync(SalaryComposition entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE pa_salary_composition
                SET
                    organization_id = @OrganizationId,
                    composition_code = @CompositionCode,
                    composition_name = @CompositionName,
                    composition_type = @CompositionType,
                    composition_nature = @CompositionNature,
                    tax_nature = @TaxNature,
                    norm_formula = @NormFormula,
                    is_allow_exceed_norm = @IsAllowExceedNorm,
                    value_type = @ValueType,
                    amount = @Amount,
                    calculation_formula = @CalculationFormula,
                    description = @Description,
                    is_display_on_payslip = @IsDisplayOnPayslip,
                    source_type = @SourceType,
                    status = @Status
                WHERE composition_id = @CompositionId;";

            return await connection.ExecuteAsync(sql, entity);
        }
    }
}
