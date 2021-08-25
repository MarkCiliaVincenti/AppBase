﻿using CarinaStudio.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CarinaStudio.AutoUpdate.Resolvers
{
	/// <summary>
	/// Tests of <see cref="XmlPackageResolver"/>.
	/// </summary>
	[TestFixture]
	class XmlPackageResolverTests : BasePackageResolverTests
	{
		// Create instance.
		protected override IPackageResolver CreateInstance(string packageManifest)
		{
			var memoryStreamProvider = new MemoryStreamProvider(Encoding.UTF8.GetBytes(packageManifest));
			return new XmlPackageResolver() { Source = memoryStreamProvider };
		}


		// Generate package manifest.
		protected override string GeneratePackageManifest(string? appName, Version? version, Uri? pageUri, IList<PackageInfo> packageInfos)
		{
			var xmlBuffer = new StringBuilder();
			using var xmlWriter = XmlWriter.Create(xmlBuffer);
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("PackageManifest");
			appName?.Let(it => xmlWriter.WriteAttributeString("Name", it));
			version?.Let(it => xmlWriter.WriteAttributeString("Version", it.ToString()));
			pageUri?.Let(it => xmlWriter.WriteAttributeString("PageUri", it.ToString()));
			foreach (var packageInfo in packageInfos)
			{
				xmlWriter.WriteStartElement("Package");
				packageInfo.Architecture?.Let(it => xmlWriter.WriteAttributeString("Architecture", it.ToString()));
				packageInfo.OperatingSystem?.Let(it => xmlWriter.WriteAttributeString("OperatingSystem", it));
				packageInfo.Uri?.Let(it => xmlWriter.WriteAttributeString("Uri", it.ToString()));
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
			return xmlBuffer.ToString();
		}
	}
}