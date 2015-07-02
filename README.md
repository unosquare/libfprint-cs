# libfprint-cs
The long-awaited C# (.net/mono) wrapper for the great fingerprint reader interfacing library libfprint

From the <a target="_blank" href="http://www.freedesktop.org/wiki/Software/fprint/libfprint/">libfprint website</a>:
<blockquote>
libfprint is an open source software library designed to make it easy for application developers to add support for consumer fingerprint readers to their software.
</blockquote>

This wrapper makes interfacing with your fingerprint reader extremely easy.

## Simple guidelines
<ul>
<li>Use the FingerprintDeviceManager to discover connected fingerprint devices</li>
<li>Use the FingerprintDevice objects provided by the device manager to Enroll, Verify and Identify fingerprints</li>
<li>Use the FingerprintGallery to keep a database of fingerprints to match against. You can load the contents of the gallery from files, a database or pretty much any byte array prodiced by the Enroll functionality.</li>
<li>Finally, (and optionally) use the PgmFormatReader to turn PGM image files to Bitmaps</li>
<li>Have fun!</li>
</ul>

## Super quick code example

```cs
using (var manager = FingerprintDeviceManager.Instance)
{
  manager.Initialize();
  using (var gallery = new FingerprintGallery())
  {
    var device = manager.DiscoverDevices().FirstOrDefault();
    device.Open();
    var enrollResult = device.EnrollFingerprint("enroll.pgm");
    if (enrollResult.IsEnrollComplete)
    {
      var isVerified = device.VerifyFingerprint(enrollResult, "verify.pgm");
      if (isVerified)
      {
        gallery.Add("username_gose_here", enrollResult);
      }
    }
    
    var identified = device.IdentifyFingerprint(gallery, "identify.pgm");
  }
}
```

Thanks to the libfprint developers!
