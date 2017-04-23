# libfprint-cs
[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/libfprint-cs/)](https://github.com/igrigorik/ga-beacon)

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

## Getting it to run on the Raspberry Pi (Raspbian)

Raspbian has a fairly outdated libfprint-dev package (0.4.0). The following script will remove the installed libfprint-dev, install dependencies, pull code from 0.5.1, build it, and install the library.

```shell
sudo apt-get remove -y libfprint-dev
sudo apt-get autoremove -y
sudo apt-get install -y libusb-1.0-0-dev
sudo apt-get install -y libnss3-dev
sudo apt-get install -y libgtk2.0-dev

wget http://people.freedesktop.org/~hadess/libfprint-0.5.1.tar.xz
tar xf libfprint-0.5.1.tar.xz
rm libfprint-0.5.1.tar.xz
cd libfprint-0.5.1
./configure
make
sudo make install
sudo make clean
cd ..
sudo cp /usr/local/lib/libfprint.so libfprint.so
sudo find / -iname "libfprint.so"
```

You will also obviously want to install mono for any of the above to work . . .
```shell
sudo apt-get install mono-complete
```

## Futures

The identification logic seems to be fairly slow when we start adding many fingerprints. Maybe using thislibrary for imaging and then using something like <a target="_blank" href="https://sourceafis.angeloflogic.com/">SourceAFIS</a> for identification would work better...

## Thanks

Thanks to the libfprint developers!
