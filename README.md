.Net (C#) Bitcoin Library
============

Collection of bitcoin related classes (based on MB300SD's Bitcoin-Tool library), can be used to impliment
a variety of functions.

Original Discription:
============

* Work with addresses, public and private keys
* Generate and sign transactions
* Connect over P2P to a bitcoin node
* Easily generate and evaulate custom scripts
* Decode and encode blocks, transactions, scripts, network messages
* Parse blockchain for data

Apps directory contains mostly complete programs that use the code.

ComputeUnspentTxOut.cs creates and updates a list of all unspent outputs. 
- Requires that the blockchain contain no orphan blocks.

ComputerAddressBalances.cs uses the unspent txout list to compute a balance of all addresses.

Use FindFirstOrphan.cs to find the file containing orphaned blocks, delete that and all subsequent 
block files and resync bitcoind.

My Story
============

As I was searching for some usable code to learn how the whole BitCoin thing was working from the inside, I stumbled upon  MB300SD's Bitcoin-Tool project (https://github.com/mb300sd/Bitcoin-Tool). As I am a .Net developer on my day time job, this was exactly the base I was looking for.

While browsing through the code and playing around with it, I started to understand what was happening and decided that instead of taking tons of notes, why not let others benefit also and start documenting into the code itself. And while doing that, also try to extend the code with the things I'm missing or would do a little bit different.

My plans with this
============

As I already said, while browsing the code I was missing a few things. These things I will try to add while documenting this project.

Currently on my wish/to-do list:
* Activity/Error logging
* Progress counters and feedback
* Configurabillity (majority of the setting are now implemented hard-coded)
* Reduce Bitcoin-QT/BitcoinD dependency (make it work in a stand-alone setup)
* Setup an API documentation page
 

As like the most of us, coding is my hobby and my life. But... Money is the thing that drives me... ;-P

So feel free to stimulate me:  1EGDuyrLuyUWfCnpEwX4LZpAuuoR7dwsdd

Used resources and licenses
============

This project:
* GPLv3 - https://www.gnu.org/licenses/gpl-3.0.html
 
Original:
* Not so clear .. See https://bitcointalk.org/index.php?topic=148163.0
* So assumed GPLv3 - https://www.gnu.org/licenses/gpl-3.0.html
  
Bouncy Castle C# Crypto API (https://www.bouncycastle.org/):
* MIT X11 License - http://www.bouncycastle.org/csharp/licence.html

Logo / Icon file:
* Joe Landy - http://www.joelandy.co.uk/
* Creative Commons Public Domain - http://creativecommons.org/publicdomain/mark/1.0/
 
