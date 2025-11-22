using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.datacowboy.SqlServerDatabaseDocumentationGenerator.Model;
using PetaPoco;

namespace net.datacowboy.SqlServerDatabaseDocumentationGenerator.Inspection
{
	class TableInspector : CommonInspector
	{
	

		public TableInspector(PetaPoco.Database petaDb):base(petaDb)
		{
				
		
		}

		public IList<Table> GetTables(Schema schema)
		{
			var tableList = this.queryForTables(schema);

			

			if (tableList != null && tableList.Count > 0)
			{
				Table table = null;

				var columnInspector = new ColumnInspector(this.peta);

				var indexInspector = new IndexInspector(this.peta);

                var foreignKeyInspector = new ForeignKeyInspector(this.peta);

				for (int i = 0; i < tableList.Count; i++)
				{
					table = tableList[i];
					table.Columns = columnInspector.GetColumns(table);
					table.Indexes = indexInspector.GetIndexes(table);
                    table.ForeignKeys = foreignKeyInspector.GetForeignKeys(table);
                    table.Parent = schema;
					table.RowCount = this.peta.ExecuteScalar<int>("Select Sum(row_count) FROM sys.dm_db_partition_stats WHERE object_id = @0 AND (index_id = 0 OR index_id = 1)", table.TableId);
					// We could store RowCount in Table class if needed in future
				}

			}


			return tableList;

		}


		private IList<Table> queryForTables(Schema schema)
		{
			var sql = new Sql(@"SELECT T.name AS TableName
								, COALESCE(EP.value, '') AS [Description]
								, T.object_id AS TableId

							FROM sys.tables AS T
								LEFT OUTER JOIN sys.extended_properties AS EP
									ON ( EP.major_id = T.object_id AND EP.minor_id = 0 AND EP.name = 'MS_Description' )

							WHERE T.schema_id = @0
 
							ORDER BY T.name", schema.SchemaId);

			return this.peta.Fetch<Table>(sql);
		}
	}
}
