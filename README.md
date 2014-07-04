Tmds.Sdp
========

This library contains a set of classes to work with Session Descriptions (RFC4566). It also contains an SAP client to discover sessions announced via the Session Announcement Protocol (RFC2974).

SessionDescription
------------------

The SessionDescription class allows to read/write/create SessionDescriptions. Once the SessionDescription is populated with data, it can be marked as read-only.

The following example, reads a SessionDescription from a file, marks it as read-only and prints out the session name.

    var sessionDescription = SessionDescription.Load(File.OpenRead("session.sdp"));
    sessionDescription.SetReadOnly();
          
    Console.Write(sessionDescription.Name);

SapClient
---------

The SapClient class detects sessions announced on the network.

The following example uses databinding to show the announced sessions in a WPF ListBox.

    SapClient sapClient = SapClient.Default;
    listBox.ItemsSource = sapClient.Sessions;
    sapClient.Enable();
