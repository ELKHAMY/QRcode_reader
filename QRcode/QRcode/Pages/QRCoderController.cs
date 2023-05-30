﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QRCodeData;
using QRCoder;

namespace pjax.Controllers
{
	public class QRCoderController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Index(string qrText)
		{
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCodeData qrCode = new global::QRCode(qrCodeData);
			Bitmap qrCodeImage = qrCode.GetGraphic(20);

			return View(BitmapToBytes(qrCodeImage));
		}

		private static Byte[] BitmapToBytes(Bitmap img)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
				return stream.ToArray();
			}
		}

		public IActionResult GenerateFile()
		{
			return View();
		}

		[HttpPost]
		public IActionResult GenerateFile(string qrText)
		{
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
			string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);
			qrCodeData.SaveRawData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCoder.QRCodeData.Compression.Uncompressed);

            QRCoder.QRCodeData qrCodeData1 = new QRCoder.QRCodeData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCoder.QRCodeData.Compression.Uncompressed);
			QRCode qrCode = new QRCode(qrCodeData1);
			Bitmap qrCodeImage = qrCode.GetGraphic(20);
			return View(BitmapToBytes(qrCodeImage));
		}

		public IActionResult ViewFile()
		{
			List<KeyValuePair<string, Byte[]>> fileData = new List<KeyValuePair<string, byte[]>>();
			KeyValuePair<string, Byte[]> data;

			string[] files = Directory.GetFiles("wwwroot/qrr");
			foreach (string file in files)
			{
                QRCoder.QRCodeData qrCodeData = new QRCoder.QRCodeData(file, QRCoder.QRCodeData.Compression.Uncompressed);
				QRCode qrCode = new QRCode(qrCodeData);
				Bitmap qrCodeImage = qrCode.GetGraphic(20);

				Byte[] byteData = BitmapToBytes(qrCodeImage);
				data = new KeyValuePair<string, Byte[]>(Path.GetFileName(file), byteData);
				fileData.Add(data);
			}

			return View(fileData);
		}
	}
}