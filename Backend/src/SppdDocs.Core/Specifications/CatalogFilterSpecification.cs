﻿using SppdDocs.Core.Entities;

namespace SppdDocs.Core.Specifications
{
	public class CatalogFilterSpecification : BaseSpecification<CatalogItem>
	{
		public CatalogFilterSpecification(int? brandId, int? typeId)
			: base(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
			            (!typeId.HasValue || i.CatalogTypeId == typeId))
		{
		}
	}
}