<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <xsl:output indent="yes" method="xml"  />
    <xsl:template match="Build">
        <Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
            <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
            <PropertyGroup>
                <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                <ProjectGuid><xsl:value-of select="@ProjectGuid"/></ProjectGuid>
                <OutputType>Exe</OutputType>
                <AppDesignerFolder>Properties</AppDesignerFolder>
                <RootNamespace><xsl:value-of select="@RootNamespace"/></RootNamespace>
                <AssemblyName><xsl:value-of select="@AssemblyName"/></AssemblyName>
                <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                <FileAlignment>512</FileAlignment>
            </PropertyGroup>
            <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                <PlatformTarget>AnyCPU</PlatformTarget>
                <DebugSymbols>true</DebugSymbols>
                <DebugType>full</DebugType>
                <Optimize>false</Optimize>
                <OutputPath>bin\Debug\</OutputPath>
                <DefineConstants>DEBUG;TRACE</DefineConstants>
                <ErrorReport>prompt</ErrorReport>
                <WarningLevel>4</WarningLevel>
            </PropertyGroup>
            <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                <PlatformTarget>AnyCPU</PlatformTarget>
                <DebugType>pdbonly</DebugType>
                <Optimize>true</Optimize>
                <OutputPath>bin\Release\</OutputPath>
                <DefineConstants>TRACE</DefineConstants>
                <ErrorReport>prompt</ErrorReport>
                <WarningLevel>4</WarningLevel>
            </PropertyGroup>
            <ItemGroup>
                <Reference Include="System" />
                <Reference Include="System.Core" />
                <Reference Include="System.Runtime.Serialization" />
                <Reference Include="System.Xml.Linq" />
                <Reference Include="System.Data.DataSetExtensions" />
                <Reference Include="Microsoft.CSharp" />
                <Reference Include="System.Data" />
                <Reference Include="System.Xml" />
            </ItemGroup>
            <ItemGroup>
                <xsl:for-each select="Include">
                    <Compile>
                        <xsl:attribute name="Include">
                            <xsl:value-of select="@Include"/>
                        </xsl:attribute>
                    </Compile>
                </xsl:for-each>
            </ItemGroup>
            <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
        </Project>
    </xsl:template>
</xsl:stylesheet>