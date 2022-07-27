using System.CommandLine;

namespace LioArch
{
    internal class CliParameters
    {
        internal static Option CustomerOption(bool required)
        {
            return new Option(
                new[] { "--customer", "-c" },
                "Specifies customer agency (default: VLOD)")
            {
                
                IsRequired = required,
                Argument = new Argument<string>(() => "VLOD"),
            };
        }

        internal static Option CustomerSectionOption(bool required)
        {
            return new Option(
                new[] { "--section", "-s" },
                "Specifies Agency Server start/stop section Name from liobase.ini")
            {
                IsRequired = required,
                Argument = new Argument<string>(),
            };
        }

        internal static Option RadioAgencyOption(bool required = false)
        {
            return new Option(
                new[] { "--radio", "-r" },
                "Specifies Radio Agency abbreviation (default: RADIO)")
            {
                IsRequired = required,
                Argument = new Argument<string>(() => "RADIO"),
            };
        }

        internal static Option RadioAgencySectionOption(bool required = false)
        {
            return new Option(
                new[] { "--radio-section", "-R" },
                @"Specifies Radio Agency start/stop section Name from liobase.ini")
            {
                IsRequired = required,
                Argument = new Argument<string>(),
            };
        }

        internal static Option VerboseOption()
        {
            return new Option(
                new[] { "--verbose", "-v" },
                @"Verbose output switch")
            {
                Argument = new Argument<bool>(() => false),
            };
        }

        internal static Option NoLioOptionDefaultFalse()
        {
            return new Option(
                "--nolio",
                @"Disables loading Lio processes")
            {
                Argument = new Argument<bool>(() => false),
            };
        }

        internal static Option AgencyOnlyOptionDefaultFalse()
        {
            return new Option(
                "--agency-only",
                @"Will setup or launch only Agency liobase.exe. Will NOT start RadioAgency")
            {
                Argument = new Argument<bool>(() => false),
            };
        }
    }
}