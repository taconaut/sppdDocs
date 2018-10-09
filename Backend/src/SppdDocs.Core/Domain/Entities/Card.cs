﻿using System;
using SppdDocs.Core.Domain.Objects;

namespace SppdDocs.Core.Domain.Entities
{
	public class Card : NamedEntity
	{
		public LocalizedText Description { get; set; }

		public Guid RarityId { get; set; }

		public Rarity Rarity { get; set; }

		public Guid ClassId { get; set; }

		public CardClass Class { get; set; }
	}
}