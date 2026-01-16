using Section2Casino;

// Parameters required by Task 2
int t = 100000; // Rolls per simulation
int n10000 = 10000;  // Number of simulations (samples)

Console.WriteLine(" Dishonest Casino Monte Carlo Simulation ");

// 1. Fair Casino
ICasino fairCasino = new FairCasino();
MonteCarloDriver fairDriver = new MonteCarloDriver(fairCasino, t);
double fairEst = fairDriver.EstimateExpectedValue(n10000);
Console.WriteLine($"Fair Casino Estimate: {fairEst:F4}");


// Dishonest Case 1 (p1=0.99, p2=0.05)
ICasino dishonest1 = new DishonestCasino(0.99, 0.05);
MonteCarloDriver driver1 = new MonteCarloDriver(dishonest1, t);
double est1 = driver1.EstimateExpectedValue(n10000);
Console.WriteLine($"Dishonest Case 1 (0.99, 0.05): {est1:F4}");


// Dishonest Case 2 (p1=0.95, p2=0.1)
ICasino dishonest2 = new DishonestCasino(0.95, 0.1);
MonteCarloDriver driver2 = new MonteCarloDriver(dishonest2, t);
double est2 = driver2.EstimateExpectedValue(n10000);
Console.WriteLine($"Dishonest Case 2 (0.95, 0.1):  {est2:F4}");


// Dishonest Case 3 (p1=0.9, p2=0.2)
ICasino dishonest3 = new DishonestCasino(0.9, 0.2);
MonteCarloDriver driver3 = new MonteCarloDriver(dishonest3, t);
double est3 = driver3.EstimateExpectedValue(n10000);
Console.WriteLine($"Dishonest Case 3 (0.9, 0.2):   {est3:F4}");




// ---------------------------------------------------------
// Task 3: Variance Analysis & Optimization

Console.WriteLine("\nTask 3: Variance Analysis (Dishonest Case 3)");

// Step 1: Estimate Variance using the existing 'dishonest3' casino
// We need individual results (Yi) to calculate variance. 
// Since the Driver only returns the mean, we run a manual loop here.
List<double> results = new List<double>();
int n_variance = 10000;

Console.WriteLine($"Collecting {n_variance} samples to estimate variance...");
for (int i = 0; i < n_variance; i++)
{
    results.Add(dishonest3.Simulate(t));
}

// Calculate Mean and Variance
double mean = results.Average();
double sumSquaredDiffs = results.Sum(val => Math.Pow(val - mean, 2));
double variance = sumSquaredDiffs / (n_variance - 1); // Formula: s^2 = Sum / (n-1)
double stdDev = Math.Sqrt(variance);

Console.WriteLine($"Estimated Variance (s^2): {variance:F6}");
Console.WriteLine($"Standard Deviation (s):   {stdDev:F6}");

// Step 2: Calculate Required Sample Size for RMSE = 0.001

double targetRMSE = 0.001;
double requiredN_calc = Math.Pow(stdDev / targetRMSE, 2);
int requiredN = (int)Math.Ceiling(requiredN_calc); // Always round up

Console.WriteLine($"\nTarget RMSE: {targetRMSE}");
Console.WriteLine($"Required Sample Size (n): {requiredN:N0}");

// Step 3: Run Final Simulation with the New Sample Size
// Now we use the MonteCarloDriver as intended for the final mean.
Console.WriteLine($"\nRunning final simulation with n={requiredN:N0}...");
double finalMean = driver3.EstimateExpectedValue(requiredN);

Console.WriteLine($"Final Expected Mean: {finalMean:F5}");



// ---------------------------------------------------------
// Section 3 Task 1: Confidence Intervals & Sample Size Optimization
// ---------------------------------------------------------
Console.WriteLine("\n--- Section 3 Task 1: Confidence Interval Analysis ---");

// Configuration for the specific problem requested:
// t = 100,000, p1 = 0.9, p2 = 0.2
ICasino casinoSec3 = new DishonestCasino(0.9, 0.2);

// 1. Define the parameters for the desired precision
double targetEpsilon = 0.0005;
double confidenceLevel = 0.99; // 99% Confidence

double GetZScore(double confLevel)
{
    
    if (confLevel >= 0.999) return 3.291;
    if (confLevel >= 0.99) return 2.576;
    if (confLevel >= 0.95) return 1.960;
    return 1.645; // Default for 0.9
}

double zScore = GetZScore(confidenceLevel);
Console.WriteLine($"Confidence Level: {confidenceLevel * 100}% (Z-Score: {zScore})");
Console.WriteLine($"Target Margin of Error (Epsilon): {targetEpsilon}");

int[] problemSizes = { 100,5000,10000, 20000, 30000 };
double estimatedStdDev = 0; // We will capture 's' from the 10k run for the formula

foreach (int n in problemSizes)
{
    Console.Write($"\nRunning simulation for n={n:N0}... ");

    // Run Simulation
    List<double> currentResults = new List<double>(n);
    for (int i = 0; i < n; i++)
    {
        currentResults.Add(casinoSec3.Simulate(t));
    }

    // Calculate Statistics
    double meanloop = currentResults.Average();
    double sumSqloop = currentResults.Sum(val => Math.Pow(val - mean, 2));
    double stdDevloop = Math.Sqrt(sumSqloop / (n - 1));

    // Calculate Confidence Interval
    double stdError = stdDevloop / Math.Sqrt(n);
    double margin = zScore * stdError;
    double lower = meanloop - margin;
    double upper = meanloop + margin;

    Console.WriteLine("Done.");
    Console.WriteLine($"  Mean: {meanloop:F5} | StdDev: {stdDevloop:F5}");
    Console.WriteLine($"  99% CI: [{lower:F5}, {upper:F5}]");
    Console.WriteLine($"  Margin of Error: {margin:F5}");

    // Capture the StdDev from the 10,000 run to use for the prediction formula below
    // (The assignment hints to use n0=10,000 for the estimate)
    if (n == 10000)
    {
        estimatedStdDev = stdDev;
    }
}

// ---------------------------------------------------------
// Calculate Required Sample Size (Optimization)
// ---------------------------------------------------------
Console.WriteLine($"\n--- Optimizing Sample Size for Target Error {targetEpsilon} ---");
Console.WriteLine($"Using estimated StdDev (s) from n=10,000 run: {estimatedStdDev:F5}");

// Formula: N = ((z * s) / Epsilon)^2
double numerator = zScore * estimatedStdDev;
double reqN_calc = Math.Pow(numerator / targetEpsilon, 2);
int requiredN_S3 = (int)Math.Ceiling(reqN_calc);

Console.WriteLine($"Calculated Required Simulations (N): {requiredN_S3:N0}");

// ---------------------------------------------------------
// Final Verification Run
// ---------------------------------------------------------
Console.WriteLine($"\nRunning FINAL simulation with n={requiredN_S3:N0}...");

List<double> finalResults = new List<double>(requiredN_S3);
for (int i = 0; i < requiredN_S3; i++)
{
    finalResults.Add(casinoSec3.Simulate(t));
}

// Calculate Final Statistics
double finalMeanSizes = finalResults.Average();
double finalSumSq = finalResults.Sum(val => Math.Pow(val - finalMeanSizes, 2));
double finalStdDev = Math.Sqrt(finalSumSq / (requiredN_S3 - 1));

double finalStdError = finalStdDev / Math.Sqrt(requiredN_S3);
double finalMargin = zScore * finalStdError;

Console.WriteLine("\nFinal Results:");
Console.WriteLine($"Mean: {finalMeanSizes:F5}");
Console.WriteLine($"99% Confidence Interval: [{finalMeanSizes - finalMargin:F5}, {finalMeanSizes + finalMargin:F5}]");
Console.WriteLine($"Actual Margin of Error: {finalMargin:F5} (Target: {targetEpsilon})");

Console.ReadLine();