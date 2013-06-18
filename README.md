# PST-Parser


A library for reading the <a href="http://msdn.microsoft.com/en-us/library/ff385210(v=office.12).aspx">PST mailstore file format</a>.

  This library is intended to be as accurate, fast implementation of the PST mailstore file format specification.  The original motivation for this project came from my experiences with other mailstore libraries that either 1) required Outlook to be installed in order to function or 2) were developed inconsistently by a third party.  Such inconsistencies range from libraries that "missed" items and other libraries that failed when encountering errors.  The intention of this project is to provide a basis to developers of applications that need to read and write to the PST format.
  
## PST Structure Overview 

  The structure of the PST file format is divided into 3 layers: NDB layer, LTP layer, and the Messaging Layer.  Each layer is implemented on top of the preceeding layer.  For example, the LTP layer may implement a heap which is stored on a node in the NDB layer.  Each layer is divided into it's own namespace.  The main entry point of parsing a PST is through the header.  In the header, information about the format and encoding is stored.  The first offsets for the NDB layer are contained Root structure in the header.
  
  The Node Database (NDB) layer layer consists of two <a href="http://en.wikipedia.org/wiki/Btree">B-trees</a>: one for nodes and another for data blocks.  Each B-tree implementation consists of intermediate blocks and leaf blocks.  The node B-tree consists of nodes that reference block IDs (BIDs) and sub nodes.  BIDs are used to traverse the data block B-tree to resolve to absolute offsets to data streams in the PST.  Data stream themselves can be in one data block or stored in another BTree if the data stream is too large to fit in one page.  XBLOCK and XXBLOCKs structures are used to store the B-trees that are used to store large data streams.
  
  The LTP layer provides the interface for the messaging layer to access properties and variable arrays of content.  The base of the LTP layer is a heap which can be stored on a node (heap-on-node or HN).  On the HN, yet another B-tree (B-tree-on-heap or BTH) is implmeneted and is used to store values on the HN using keys.  The BTH (can be thought of just as a heap) is used to store Property Contexts (PCs) and Table Contexts (TCs).  
  
  The messaging layer uses the LTP layer to represent folder heirarchies and the messages that exist in a give folder.
  
## Future Plans (finish it)
  
  Currently, as most open source projects, there are several incomplete features.  The two biggest of such features are the ability to write to a PST and implementation of the ANSI format of PST files.  Writing to PST file will require much more work than has already been completed.  Supporting the ability to write includes adding implementation of parsing the various allocation tables, updating reference counts when removing or adding new blocks, and recalculating new CRC values if necessary.  Forunately, instructions for accomplishing this are provided by the PST file format specfiication.  Once these features are implemented, work can focus on exposing an API from the message layer.  A sample application is included to demonstrate some of the functionality (the location of the PST is hard coded, so you will have to provide your own).

