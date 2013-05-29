// **************************************************************************
// Copyright 2007 - 2009 Tavs Dokkedahl
// Contact: http://www.jslab.dk/contact.php
//
// This file is part of the JSLab Standard Library (JSL) Program.
//
// JSL is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// any later version.
//
// JSL is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ***************************************************************************
// File created 2010-11-16 05:30:42

// Name of the months
Date.nameOfMonths = ['January','February','March','April','May','June','July','August','September','October','November','December'];

// Format a date according to a specified format
Date.prototype.format =
  function(s,utc) {
    // Split into array
    s = s.split('');
    var l = s.length;
    var r = '';
    var n = m = null;
    for (var i=0; i<l; i++) {
      switch(s[i]) {
        // Day of the month, 2 digits with leading zeros: 01 to 31
        case 'd':
          n = utc ? this.getUTCDate() : this.getDate();
          if (n * 1 < 10)
            r += '0';
          r += n;
          break;
        // A textual representation of a day, three letters:  Mon through Sun 
        case 'D':
          r += this.getNameOfDay(utc).substring(0,3);
          break;
        // Day of the month without leading zeros:   1 to 31
        case 'j':
          r += utc ? this.getUTCDate() : this.getDate();
          break;
        // Lowercase l A full textual representation of the day of the week: Sunday (0) through Saturday (6) 
        case 'l':
          r += this.getNameOfDay(utc);
          break;
        // ISO-8601 numeric representation of the day of the week: 1 (for Monday) through 7 (for Sunday) 
        case 'N':
          r += this.getISODay(utc);
          break;
        // English ordinal suffix for the day of the month, 2 characters
        case 'S':
          r += this.getDaySuffix(utc);
          break;
        // Numeric representation of the day of the week: 0 (for Sunday) through 6 (for Saturday) 
        case 'w':
          r += utc ? this.getUTCDay() : this.getDay();
          break;
        // The day of the year (starting from 0) 0 through 365
        case 'z':
          n = 0;
          m = utc ? this.getUTCMonth() : this.getMonth();
          for(var i=0; i<m; i++)
            n += Date.daysInMonth[i]
          if (this.isLeapYear())
            n++;
          n += utc ? this.getUTCDate() : this.getDate();
          n--;
          r += n;
          break;
        //   break;
        // ISO-8601 week number of year, weeks starting on Monday
        case 'W':
          r += this.getISOWeek(utc);
          break;
        // A full textual representation of a month, such as January or March:  January through December 
        case 'F':
          r += this.getNameOfMonth(utc);
          break;
        // Numeric representation of a month, with leading zeros 01 through 12 
        case 'm':
          n = utc ? this.getUTCMonth() : this.getMonth();
          n++;
          if (n < 10)
            r += '0';
          r += n;
          break;
        // A short textual representation of a month, three letters:  Jan through Dec 
        case 'M':
          r += this.getNameOfMonth(utc).substring(0,3);
          break;
        // Numeric representation of a month, without leading zeros:  1 through 12 
        case 'n':
          n = utc ? this.getUTCMonth() : this.getMonth();
           r += ++n;
          break;
        // Number of days in the given month: 28 through 31 
        case 't':
          r += this.getDaysInMonth(utc);
          break;
        // Whether it's a leap year:  1 if it is a leap year, 0 otherwise.
        case 'L':
          if (this.isLeapYear(utc))
            r += '1';
          else
            r += '0';
          break;
        // ISO-8601 year number. This has the same value as Y, except that if the ISO week number (W) belongs to the previous or next year, that year is used instead
        /*
        case 'o':
          break;
        */
        // A full numeric representation of a year, 4 digits
        case 'Y':
          r += utc ? this.getUTCFullYear() : this.getFullYear();
          break;
        // A two digit representation of a year
        case 'y':
          n = utc ? this.getUTCFullYear() : this.getFullYear();
          r += (n + '').substring(2);
          break;
        // Lowercase Ante meridiem and Post meridiem am or pm 
        case 'a':
          n = utc ? this.getUTCHours() : this.getHours();
          r += n < 12 ? 'am' : 'pm';
          break;
        // Uppercase Ante meridiem and Post meridiem AM or PM 
        case 'A':
          n = utc ? this.getUTCHours() : this.getHours();
          r += n < 12 ? 'AM' : 'PM';
          break;
        // Swatch Internet time 000 through 999 
        // case 'B':
        //   break;
        // 12-hour format of an hour without leading zeros
        case 'g':
          n = utc ? this.getUTCHours() : this.getHours();
          if (n > 12)
            n -= 12;
          r += n;
          break;
        // 24-hour format of an hour without leading zeros 0 through 23
        case 'G':
          r += this.getHours();
          break;
        //  12-hour format of an hour with leading zeros 01 through 12 
        case 'h':
          n = utc ? this.getUTCHours() : this.getHours();
          if (n > 12)
            n -= 12;
          if (n < 10)
            r += '0';
          r += n;
          break;
        // 24-hour format of an hour with leading zeros 00 through 23 
        case 'H':
          n = utc ? this.getUTCHours() : this.getHours();
          if (n < 10)
            r += '0';
          r += n;
          break;
        // i Minutes with leading zeros 00 to 59 
        case 'i':
          n = utc ? this.getUTCMinutes() : this.getMinutes();
          if (n < 10)
            r += '0';
          r += n;
          break;
        // s Seconds, with leading zeros 00 through 59 
        case 's':
          n = utc ? this.getUTCSeconds() : this.getSeconds();
          if (n < 10)
            r += '0';
          r += n;
          break;
        // Milliseconds
        case 'u':
          r += utc ? this.getUTCMilliseconds() : this.getMilliseconds();
          break;
        // Timezone identifier
        // case 'e':
        //   break;
        // Whether or not the date is in daylight saving time 1 if Daylight Saving Time, 0 otherwise. 
        case 'I':
          if (this.getMinutes() != this.getUTCMinutes)
            r += '1';
          else
            r += '0';
          break;
        // Difference to Greenwich time (GMT) in hours
        case 'O':
          n = this.getTimezoneOffset() / 60;
          if (n >= 0)
            r += '+';
          else
            r += '-';
          n = Math.abs(n);
          if (Math.abs(n) < 10)
            r += '0';
           r += n + '00';
          break;
        // Difference to Greenwich time (GMT) with colon between hours and minutes: Example: +02:00 
        case 'P':
          n = this.getTimezoneOffset() / 60;
          if (n >= 0)
            r += '+';
          else
            r += '-';
          n = Math.abs(n);
          if (Math.abs(n) < 10)
            r += '0';
           r += n + ':00';
          break;
        // T Timezone abbreviation EST, MDT etc. 
        // case 'T':
        //   break;
        // Z Timezone offset in seconds. The offset for timezones west of UTC is always negative, and for those east of UTC is always positive. 
        case 'Z':
          r += this.getTimezoneOffset() * 60;
          break;
        // ISO 8601 date: 2004-02-12T15:19:21+00:00 
        case 'c':
          r += this.format('Y-m-d',utc) + 'T' + this.format('H:i:sP',utc);
          break;
        // RFC 2822 formatted date Example: Thu, 21 Dec 2000 16:01:07 +0200 
        case 'r':
          r += this.format('D, j M Y H:i:s P',utc);
          break;
        case 'U':
          r += this.getTime();
          break;
        default:
          r += s[i];
      }
    }
    return r
  };

// Gen the english suffix for dates
Date.prototype.getDaySuffix =
  function(utc) {
    var n = utc ? this.getUTCDate() : this.getDate();
    // If not the 11th and date ends at 1
    if (n != 11 && (n + '').match(/1$/))
      return 'st';
    // If not the 12th and date ends at 2
    else if (n != 12 && (n + '').match(/2$/))
      return 'nd';
    // If not the 13th and date ends at 3
    else if (n != 13 && (n + '').match(/3$/))
      return 'rd';
    else
      return 'th';
  };

// Return the ISO day number for a date
Date.prototype.getISODay =
  function(utc) {
    // Native JS method - Sunday is 0, monday is 1 etc.
    var d = utc ? this.getUTCDay() : this.getDay();
    // Return d if not sunday; otherwise return 7
    return d ? d : 7;
  };

// Get ISO week number of the year
// The algorithm is credit to Claus Tøndering and is taken from his calendar FAQ
// See http://www.tondering.dk/claus/cal/node8.html#SECTION00880000000000000000
// for more information
// Integer division: a/b|0
Date.prototype.getISOWeek =
  function(utc) {
    var y = utc ? this.getUTCFullYear(): this.getFullYear();
    var m = utc ? this.getUTCMonth() + 1: this.getMonth() + 1;
    var d = utc ? this.getUTCDate() : this.getDate();
    // If month jan. or feb.
    if (m < 3) {
      var a = y - 1;
      var b = (a / 4 | 0) - (a / 100 | 0) + (a / 400 | 0);
      var c = ( (a - 1) / 4 | 0) - ( (a - 1) / 100 | 0) + ( (a - 1) / 400 | 0);
      var s = b - c;
      var e = 0;
      var f = d - 1 + 31 * (m - 1);
    }
    // If month mar. through dec.
    else {
      var a = y;
      var b = (a / 4 | 0) - ( a / 100 | 0) + (a / 400 | 0);
      var c = ( (a - 1) / 4 | 0) - ( (a - 1) / 100 | 0) + ( (a - 1) / 400 | 0);
      var s = b - c;
      var e = s + 1;
      var f = d + ( (153 * (m - 3) + 2) / 5 | 0) + 58 + s;
    }
    var g = (a + b) % 7;
    // ISO Weekday (0 is monday, 1 is tuesday etc.)
    var d = (f + g - e) % 7;
    var n = f + 3 - d;
    if (n < 0)
      var w = 53 - ( (g - s) / 5 | 0);
    else if (n > 364 + s)
      var w = 1;
    else
      var w = (n / 7 | 0) + 1;
    return w;
  };

// Return the name of the weekday
Date.prototype.getNameOfDay =
  function(utc) {
    var d = this.getISODay(utc) - 1;
    return Date.nameOfDays[d];
  };

// Return the name of the month
Date.prototype.getNameOfMonth =
  function(utc) {
    var m = utc ? this.getUTCMonth() : this.getMonth();
    return Date.nameOfMonths[m];
  };

// Rewrite native Date.getTimezoneOffset to return values with correct sign
Date.prototype._getTimezoneOffset = Date.prototype.getTimezoneOffset;
Date.prototype.getTimezoneOffset =
  function() {
    return this._getTimezoneOffset() * -1;
  };

// Returns true if year is a leap year; otherwise false
Date.prototype.isLeapYear =
  function(utc) {
    var y = utc ? this.getUTCFullYear() : this.getFullYear();
    return !(y % 4) && (y % 100) || !(y % 400) ? true : false;
  };
  
// Return date after add days
Date.prototype.addDays = function(days) {
this.setDate(this.getDate()+days);
} 

Date.prototype.dayDiff = function(d2) {
    var d= Math.abs(this-d2);
    return Math.floor(d/(24*60*60*1000));
}
 
Date.prototype.busDayDiff = function(d2) {
    var d = this.dayDiff(d2); var t= d%7;
    if (this < d2) {w1= this.getDay(); w2= d2.getDay();}
    else {w2= this.getDay(); w1= d2.getDay();}
    if (w1 > w2) t-=2;if ((w1 == 0 && w2 == 6) || w1 == 6) t--;
    return Math.abs((Math.floor(d/7)*5)+t);
}

// Return a random integer in the interval [0,n]
Math.randomN =
  function(n) {
    return (Math.random() * (n + 1)) | 0;
  };

// Return true if number is a float
Number.prototype.isFloat =
      function() {
        return /\./.test(this.toString());
      };

// Return true if number is an integer
Number.prototype.isInteger =
      function() {
        return Math.floor(this) == this ? true : false;
      };

// Return the hexidecimal string representation of an integer
    Number.prototype.toHex =
      function() {
        // Only convert integers
        if (!this.isInteger())
          throw new Error('Number is not an integer');
        // Can't assign to 'this' so we must copy
        var d = this;
        // Quotient and remainder
        var q = r = null
        // Return value
        var s = '';
        do {
          q = Math.floor(d / 16);
          r = d % 16;
          // If r > 9 then get correct letter (A-F)
          s = r < 10 ? r + s : String.fromCharCode(r + 55) + s;
          d = q;
        }
        while(q)
        return s;
      };

// Convert HTML breaks to newline
String.prototype.br2nl =
  function() {
    return this.replace(/<br\s*\/?>/mg,"\n");
  };

/*
Calculate the Levensthein distance (LD) of two strings
Algorithm is taken from http://www.merriampark.com/ld.htm. The algorithm is considered to be public domain.
1
  Set n to be the length of s.
  Set m to be the length of t.
  If n = 0, return m and exit.
  If m = 0, return n and exit.
  Construct a matrix containing 0..m rows and 0..n columns.  
2 
  Initialize the first row to 0..n.
  Initialize the first column to 0..m.
3
  Examine each character of s (i from 1 to n). 
4
  Examine each character of t (j from 1 to m). 
5
  If s[i] equals t[j], the cost is 0.
  If s[i] doesn't equal t[j], the cost is 1. 
6
  Set cell d[i,j] of the matrix equal to the minimum of:
  a. The cell immediately above plus 1: d[i-1,j] + 1.
  b. The cell immediately to the left plus 1: d[i,j-1] + 1.
  c. The cell diagonally above and to the left plus the cost: d[i-1,j-1] + cost.
7
  After the iteration steps (3, 4, 5, 6) are complete, the distance is found in cell d[n,m].
*/
String.prototype.levenshtein=
  function(t) {
    // ith character of s
    var si; 
    // cost
    var c;
    // Step 1
    var n = this.length;
    var m = t.length;
    if (!n)
      return m;
    if (!m)
      return n;
    // Matrix
    var mx = [];
    // Step 2 - Init matrix
    for (var i=0; i<=n; i++) {
      mx[i] = [];
      mx[i][0] = i;
    }
    for (var j=0; j<=m; j++)
      mx[0][j] = j;
    // Step 3 - For each character in this
    for (var i=1; i<=n; i++) {
      si = this.charAt(i - 1);
      // Step 4 - For each character in t do step 5 (si == t.charAt(j - 1) ? 0 : 1) and 6
      for (var j=1; j<=m; j++)
        mx[i][j] = Math.min(mx[i - 1][j] + 1, mx[i][j - 1] + 1, mx[i - 1][j - 1] + (si == t.charAt(j - 1) ? 0 : 1));
    }
    // Step 7
    return mx[n][m];
  };

// Convert HTML whitespace to normal whitespace
String.prototype.nbsp2s =
  function() {
    return this.replace(/ ?/mg," ");
  };

// Count the number of times a substring is in a string.
String.prototype.substrCount =
  function(s) {
    return this.length && s ? (this.split(s)).length - 1 : 0;
  };

// Return a new string without leading and trailing whitespace
// Double spaces whithin the string are removed as well
String.prototype.trimAll =
  function() {
    return this.replace(/^\s+|(\s+(?!\S))/mg,"");
  };

// Return a new string without leading whitespace
String.prototype.trimLeft =
  function() {
    return this.replace(/^\s+/,"");
  };

// Return a new string without trailing whitespace
String.prototype.trimRight =
  function() {
    return this.replace(/\s+$/,"");
  };

// Names of the week days
Date.nameOfDays = ['Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday'];

// Return a new string without leading and trialing whitespace
String.prototype.trim =
  function() {
    return this.replace(/^\s*?(\S*?)\s*$/,'$1');
  };

String.format = function (text)
{
    //check if there are two arguments in the arguments list
    if ( arguments.length <= 1 )
    {
        //if there are not 2 or more arguments there's nothing to replace
        //just return the original text
        return text;
    }
    //decrement to move to the second argument in the array
    var tokenCount = arguments.length - 2;
    for( var token = 0; token <= tokenCount; token++ )
    {
        //iterate through the tokens and replace their placeholders from the original text in order
        text = text.replace( new RegExp( "\\{" + token + "\\}", "gi" ), arguments[ token + 1 ] );
    }
    return text;
};
